using Virtus.Domain.Enums;
using Virtus.Domain.ValueObjects;
using Virtus.Domain.Exceptions;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade base para representar uma pessoa no sistema
/// </summary>
public class Pessoa : BaseEntity
{
  public string Nome { get; private set; } = default!;
  public Email? Email { get; private set; }
  public Telefone? Telefone { get; private set; }
  public CPF? CPF { get; private set; }
  public TipoPessoa Tipo { get; private set; }
  public bool Ativo { get; private set; }

  protected Pessoa() { }

  public Pessoa(string nome, TipoPessoa tipo, string? email = null, string? telefone = null, string? cpf = null)
  {
    DefinirNome(nome);
    Tipo = tipo;
    Ativo = true;

    if (!string.IsNullOrWhiteSpace(email))
      DefinirEmail(email);

    if (!string.IsNullOrWhiteSpace(telefone))
      DefinirTelefone(telefone);

    if (!string.IsNullOrWhiteSpace(cpf))
      DefinirCPF(cpf);
  }

  /// <summary>
  /// Define o nome da pessoa com validação
  /// </summary>
  public void DefinirNome(string nome)
  {
    if (string.IsNullOrWhiteSpace(nome))
      throw new ValidationException("Nome é obrigatório");

    if (nome.Length < 3)
      throw new ValidationException("Nome deve ter pelo menos 3 caracteres");

    if (nome.Length > 100)
      throw new ValidationException("Nome não pode ter mais de 100 caracteres");

    Nome = nome.Trim();
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Define o email da pessoa
  /// </summary>
  public void DefinirEmail(string email)
  {
    Email = new Email(email);
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Define o telefone da pessoa
  /// </summary>
  public void DefinirTelefone(string telefone)
  {
    Telefone = new Telefone(telefone);
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Define o CPF da pessoa
  /// </summary>
  public void DefinirCPF(string cpf)
  {
    CPF = new CPF(cpf);
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Ativa a pessoa no sistema
  /// </summary>
  public void Ativar()
  {
    Ativo = true;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Desativa a pessoa no sistema
  /// </summary>
  public void Desativar()
  {
    Ativo = false;
    DefinirDataAtualizacao();
  }
}