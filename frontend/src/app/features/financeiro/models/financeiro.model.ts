export interface Mensalidade {
  id: string;
  matriculaId: string;
  competencia: string; // YYYY-MM
  vencimento: string;
  valor: number;
  valorPago: number;
  status: StatusMensalidade;
  dataPagamento?: string;
  observacao?: string;
  matricula?: {
    id: string;
    aluno: {
      id: string;
      nome: string;
      cpf: string;
    };
    turma: {
      id: string;
      nome: string;
      curso: {
        id: string;
        nome: string;
      };
    };
  };
}

export type StatusMensalidade =
  | 'EmAberto'
  | 'PagoParcial'
  | 'Pago'
  | 'Atrasado'
  | 'Cancelado';

export interface Pagamento {
  id: string;
  responsavelId: string;
  valor: number;
  dataPagamento: string;
  meioPagamento: MeioPagamento;
  comprovante?: string;
  observacao?: string;
  saldoUtilizado: number;
  saldoGerado: number;
  parcelas: PagamentoParcela[];
  responsavel?: {
    id: string;
    nome: string;
    cpf: string;
  };
}

export type MeioPagamento =
  | 'Pix'
  | 'Dinheiro'
  | 'Cartao'
  | 'Boleto'
  | 'Transferencia'
  | 'Outro';

export interface PagamentoParcela {
  id: string;
  pagamentoId: string;
  mensalidadeId: string;
  valorAlocado: number;
  mensalidade?: Mensalidade;
}

export interface CreatePagamentoRequest {
  responsavelId: string;
  valor: number;
  dataPagamento: string;
  meioPagamento: MeioPagamento;
  comprovante?: string;
  observacao?: string;
  alocacoes: AlocacaoMensalidade[];
}

export interface AlocacaoMensalidade {
  mensalidadeId: string;
  valor: number;
}

export interface SaldoResponsavel {
  responsavelId: string;
  saldo: number;
  ultimaAtualizacao: string;
}

export interface ExtratoResponsavel {
  responsavelId: string;
  saldoAtual: number;
  movimentacoes: MovimentacaoSaldo[];
}

export interface MovimentacaoSaldo {
  id: string;
  data: string;
  tipo: 'Credito' | 'Debito';
  valor: number;
  descricao: string;
  saldoApos: number;
}

// Helpers para conciliação
export interface MensalidadeParaAlocar {
  mensalidade: Mensalidade;
  valorPendente: number;
  valorAlocado: number;
  selecionada: boolean;
}

export interface ConfiguracaoAlocacao {
  estrategia: 'automatica' | 'manual';
  priorizarMaisAntigos: boolean;
  considerarSaldo: boolean;
}
