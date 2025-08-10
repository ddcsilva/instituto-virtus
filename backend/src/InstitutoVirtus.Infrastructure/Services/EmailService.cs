using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace InstitutoVirtus.Infrastructure.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
    Task SendBulkEmailAsync(List<string> recipients, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
    Task SendTemplateEmailAsync(string to, string templateName, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _useSsl;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        var emailSection = _configuration.GetSection("Email");

        // Para desenvolvimento, usar Ethereal Email (serviço gratuito de teste)
        // Criar conta em: https://ethereal.email/
        _smtpHost = emailSection["SmtpHost"] ?? "smtp.ethereal.email";
        _smtpPort = int.Parse(emailSection["SmtpPort"] ?? "587");
        _smtpUser = emailSection["SmtpUser"] ?? "your.ethereal.user@ethereal.email";
        _smtpPassword = emailSection["SmtpPassword"] ?? "your-ethereal-password";
        _fromEmail = emailSection["From"] ?? "noreply@institutovirtus.com.br";
        _fromName = emailSection["FromName"] ?? "Instituto Virtus";
        _useSsl = bool.Parse(emailSection["UseSsl"] ?? "true");
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var builder = new BodyBuilder();

            if (isHtml)
            {
                builder.HtmlBody = WrapInTemplate(body);
                builder.TextBody = StripHtml(body);
            }
            else
            {
                builder.TextBody = body;
            }

            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync(_smtpUser, _smtpPassword, cancellationToken);
            await smtp.SendAsync(message, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation($"Email enviado para {to}: {subject}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao enviar email para {to}");
            throw;
        }
    }

    public async Task SendBulkEmailAsync(List<string> recipients, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        // Para produção, usar serviço como SendGrid que suporta envio em massa
        // Por enquanto, enviar individualmente com delay
        foreach (var recipient in recipients)
        {
            await SendEmailAsync(recipient, subject, body, isHtml, cancellationToken);
            await Task.Delay(1000, cancellationToken); // Delay para evitar rate limiting
        }
    }

    public async Task SendTemplateEmailAsync(string to, string templateName, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
    {
        var template = GetEmailTemplate(templateName);

        foreach (var param in parameters)
        {
            template = template.Replace($"{{{{{param.Key}}}}}", param.Value);
        }

        await SendEmailAsync(to, GetSubjectForTemplate(templateName), template, true, cancellationToken);
    }

    private string WrapInTemplate(string content)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <style>
                body {{
                    font-family: 'Arial', sans-serif;
                    line-height: 1.6;
                    color: #333;
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                }}
                .header {{
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    padding: 30px;
                    text-align: center;
                    border-radius: 10px 10px 0 0;
                }}
                .content {{
                    background: white;
                    padding: 30px;
                    border: 1px solid #e0e0e0;
                    border-radius: 0 0 10px 10px;
                }}
                .footer {{
                    text-align: center;
                    padding: 20px;
                    color: #666;
                    font-size: 12px;
                }}
                .button {{
                    display: inline-block;
                    padding: 12px 30px;
                    background: #667eea;
                    color: white;
                    text-decoration: none;
                    border-radius: 5px;
                    margin: 20px 0;
                }}
            </style>
        </head>
        <body>
            <div class='header'>
                <h1>Instituto Virtus</h1>
                <p>Educação Musical e Teológica de Excelência</p>
            </div>
            <div class='content'>
                {content}
            </div>
            <div class='footer'>
                <p>© 2025 Instituto Virtus. Todos os direitos reservados.</p>
                <p>Este é um email automático. Por favor, não responda.</p>
            </div>
        </body>
        </html>";
    }

    private string StripHtml(string html)
    {
        // Simples remoção de tags HTML
        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
    }

    private string GetEmailTemplate(string templateName)
    {
        return templateName switch
        {
            "BemVindo" => @"
                <h2>Bem-vindo ao Instituto Virtus!</h2>
                <p>Olá {{Nome}},</p>
                <p>É com grande alegria que recebemos você em nossa instituição!</p>
                <p>Seu cadastro foi realizado com sucesso.</p>
                <p><strong>Seus dados de acesso:</strong></p>
                <ul>
                    <li>Email: {{Email}}</li>
                    <li>Senha temporária: {{SenhaTemporia}}</li>
                </ul>
                <p>Por segurança, recomendamos que altere sua senha no primeiro acesso.</p>
                <a href='{{LinkAcesso}}' class='button'>Acessar Portal</a>
                <p>Atenciosamente,<br>Equipe Instituto Virtus</p>",

            "MatriculaConfirmada" => @"
                <h2>Matrícula Confirmada!</h2>
                <p>Olá {{NomeResponsavel}},</p>
                <p>Confirmamos a matrícula de <strong>{{NomeAluno}}</strong> no curso de <strong>{{NomeCurso}}</strong>.</p>
                <p><strong>Detalhes da Turma:</strong></p>
                <ul>
                    <li>Professor(a): {{NomeProfessor}}</li>
                    <li>Horário: {{Horario}}</li>
                    <li>Sala: {{Sala}}</li>
                    <li>Início das aulas: {{DataInicio}}</li>
                </ul>
                <p>Valor da mensalidade: <strong>R$ {{ValorMensalidade}}</strong></p>
                <p>Vencimento: Todo dia {{DiaVencimento}}</p>
                <p>Qualquer dúvida, entre em contato conosco.</p>
                <p>Atenciosamente,<br>Equipe Instituto Virtus</p>",

            "LembreteVencimento" => @"
                <h2>Lembrete de Vencimento</h2>
                <p>Olá {{NomeResponsavel}},</p>
                <p>Este é um lembrete amigável sobre a mensalidade de <strong>{{NomeAluno}}</strong>.</p>
                <p><strong>Detalhes:</strong></p>
                <ul>
                    <li>Competência: {{Competencia}}</li>
                    <li>Valor: R$ {{Valor}}</li>
                    <li>Vencimento: {{DataVencimento}}</li>
                </ul>
                <p>Para sua comodidade, você pode realizar o pagamento via PIX:</p>
                <p><strong>Chave PIX:</strong> {{ChavePix}}</p>
                <a href='{{LinkPortal}}' class='button'>Acessar Portal</a>
                <p>Atenciosamente,<br>Equipe Instituto Virtus</p>",

            "ReciboPagamento" => @"
                <h2>Recibo de Pagamento</h2>
                <p>Olá {{NomeResponsavel}},</p>
                <p>Confirmamos o recebimento do seu pagamento.</p>
                <p><strong>Detalhes do Pagamento:</strong></p>
                <ul>
                    <li>Data: {{DataPagamento}}</li>
                    <li>Valor: R$ {{ValorTotal}}</li>
                    <li>Forma de Pagamento: {{FormaPagamento}}</li>
                    <li>Referência: {{Referencia}}</li>
                </ul>
                <p><strong>Mensalidades Quitadas:</strong></p>
                {{ListaMensalidades}}
                <p>Agradecemos pela pontualidade!</p>
                <p>Atenciosamente,<br>Equipe Instituto Virtus</p>",

            "ResetSenha" => @"
                <h2>Redefinição de Senha</h2>
                <p>Olá {{Nome}},</p>
                <p>Recebemos uma solicitação para redefinir sua senha.</p>
                <p>Se você não solicitou esta alteração, ignore este email.</p>
                <p>Para criar uma nova senha, clique no botão abaixo:</p>
                <a href='{{LinkReset}}' class='button'>Redefinir Senha</a>
                <p>Este link expira em 24 horas.</p>
                <p>Atenciosamente,<br>Equipe Instituto Virtus</p>",

            _ => "<p>Template não encontrado</p>"
        };
    }

    private string GetSubjectForTemplate(string templateName)
    {
        return templateName switch
        {
            "BemVindo" => "Bem-vindo ao Instituto Virtus!",
            "MatriculaConfirmada" => "Matrícula Confirmada - Instituto Virtus",
            "LembreteVencimento" => "Lembrete de Vencimento - Instituto Virtus",
            "ReciboPagamento" => "Recibo de Pagamento - Instituto Virtus",
            "ResetSenha" => "Redefinição de Senha - Instituto Virtus",
            _ => "Instituto Virtus"
        };
    }
}