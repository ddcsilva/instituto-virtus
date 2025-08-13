import { Injectable, computed } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { tapResponse } from '@ngrx/operators';
import { Observable, switchMap, tap, combineLatest, map } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import {
  Mensalidade,
  Pagamento,
  CreatePagamentoRequest,
  SaldoResponsavel,
  ExtratoResponsavel,
  MensalidadeParaAlocar,
  ConfiguracaoAlocacao,
  AlocacaoMensalidade,
} from '../models/financeiro.model';
import { FinanceiroService, MensalidadeFilter, PagedResult } from './financeiro.service';

export interface FinanceiroState {
  mensalidades: Mensalidade[];
  mensalidadesParaAlocar: MensalidadeParaAlocar[];
  pagamentos: Pagamento[];
  selectedPagamento: Pagamento | null;
  saldoResponsavel: SaldoResponsavel | null;
  extrato: ExtratoResponsavel | null;
  configuracaoAlocacao: ConfiguracaoAlocacao;
  loading: boolean;
  error: string | null;
  filter: MensalidadeFilter;
  total: number;
}

@Injectable()
export class FinanceiroStore extends ComponentStore<FinanceiroState> {
  constructor(
    private readonly financeiroService: FinanceiroService,
    private readonly snackBar: MatSnackBar
  ) {
    super({
      mensalidades: [],
      mensalidadesParaAlocar: [],
      pagamentos: [],
      selectedPagamento: null,
      saldoResponsavel: null,
      extrato: null,
      configuracaoAlocacao: {
        estrategia: 'automatica',
        priorizarMaisAntigos: true,
        considerarSaldo: true,
      },
      loading: false,
      error: null,
      filter: { page: 0, pageSize: 10 },
      total: 0,
    });
  }

  // Selectors
  readonly mensalidades$ = this.select(state => state.mensalidades);
  readonly mensalidadesParaAlocar$ = this.select(state => state.mensalidadesParaAlocar);
  readonly pagamentos$ = this.select(state => state.pagamentos);
  readonly saldoResponsavel$ = this.select(state => state.saldoResponsavel);
  readonly loading$ = this.select(state => state.loading);
  readonly configuracaoAlocacao$ = this.select(state => state.configuracaoAlocacao);

  readonly totalAlocado$ = this.select(this.mensalidadesParaAlocar$, mensalidades =>
    mensalidades.reduce((acc, m) => acc + m.valorAlocado, 0)
  );

  readonly totalPendente$ = this.select(this.mensalidadesParaAlocar$, mensalidades =>
    mensalidades.filter(m => m.selecionada).reduce((acc, m) => acc + m.valorPendente, 0)
  );

  readonly mensalidadesEmAberto$ = this.select(this.mensalidades$, mensalidades =>
    mensalidades.filter(m => m.status === 'EmAberto' || m.status === 'Atrasado')
  );

  // Updaters
  readonly setLoading = this.updater((state, loading: boolean) => ({
    ...state,
    loading,
  }));

  readonly setMensalidades = this.updater((state, result: PagedResult<Mensalidade>) => ({
    ...state,
    mensalidades: result.items,
    total: result.total,
    loading: false,
  }));

  readonly setMensalidadesParaAlocar = this.updater((state, mensalidades: Mensalidade[]) => ({
    ...state,
    mensalidadesParaAlocar: mensalidades.map(m => ({
      mensalidade: m,
      valorPendente: m.valor - m.valorPago,
      valorAlocado: 0,
      selecionada: false,
    })),
  }));

  readonly setPagamentos = this.updater((state, result: PagedResult<Pagamento>) => ({
    ...state,
    pagamentos: result.items,
    loading: false,
  }));

  readonly setSaldoResponsavel = this.updater((state, saldo: SaldoResponsavel) => ({
    ...state,
    saldoResponsavel: saldo,
  }));

  readonly setConfiguracaoAlocacao = this.updater(
    (state, config: Partial<ConfiguracaoAlocacao>) => ({
      ...state,
      configuracaoAlocacao: { ...state.configuracaoAlocacao, ...config },
    })
  );

  readonly toggleMensalidadeSelecao = this.updater((state, mensalidadeId: string) => ({
    ...state,
    mensalidadesParaAlocar: state.mensalidadesParaAlocar.map(m =>
      m.mensalidade.id === mensalidadeId ? { ...m, selecionada: !m.selecionada } : m
    ),
  }));

  readonly atualizarValorAlocado = this.updater(
    (state, update: { mensalidadeId: string; valor: number }) => ({
      ...state,
      mensalidadesParaAlocar: state.mensalidadesParaAlocar.map(m =>
        m.mensalidade.id === update.mensalidadeId
          ? { ...m, valorAlocado: Math.min(update.valor, m.valorPendente) }
          : m
      ),
    })
  );

  // Effects
  readonly loadMensalidades = this.effect((filter$: Observable<MensalidadeFilter>) =>
    filter$.pipe(
      tap(() => this.setLoading(true)),
      switchMap(filter =>
        this.financeiroService.getMensalidades(filter).pipe(
          tapResponse(
            result => this.setMensalidades(result),
            error => {
              this.snackBar.open('Erro ao carregar mensalidades', 'Fechar', {
                duration: 3000,
              });
            }
          )
        )
      )
    )
  );

  readonly loadMensalidadesResponsavel = this.effect((responsavelId$: Observable<string>) =>
    responsavelId$.pipe(
      tap(() => this.setLoading(true)),
      switchMap(responsavelId =>
        this.financeiroService.getMensalidadesResponsavel(responsavelId).pipe(
          tapResponse(
            mensalidades => {
              this.setMensalidadesParaAlocar(mensalidades);
              this.setLoading(false);
            },
            () => {
              this.snackBar.open('Erro ao carregar mensalidades do responsável', 'Fechar', {
                duration: 3000,
              });
            }
          )
        )
      )
    )
  );

  readonly loadPagamentos = this.effect((filter$: Observable<any>) =>
    filter$.pipe(
      tap(() => this.setLoading(true)),
      switchMap(filter =>
        this.financeiroService.getPagamentos(filter).pipe(
          tapResponse(
            result => this.setPagamentos(result),
            error => {
              this.snackBar.open('Erro ao carregar pagamentos', 'Fechar', {
                duration: 3000,
              });
            }
          )
        )
      )
    )
  );

  readonly criarPagamento = this.effect((request$: Observable<CreatePagamentoRequest>) =>
    request$.pipe(
      tap(() => this.setLoading(true)),
      switchMap(request =>
        this.financeiroService.criarPagamento(request).pipe(
          tapResponse(
            pagamento => {
              this.snackBar.open('Pagamento registrado com sucesso', 'Fechar', { duration: 3000 });
              this.setLoading(false);
            },
            error => {
              this.snackBar.open('Erro ao registrar pagamento', 'Fechar', {
                duration: 3000,
              });
            }
          )
        )
      )
    )
  );

  // Métodos de alocação
  alocarAutomaticamente(valorTotal: number): void {
    const state = this.get();
    const mensalidades = [...state.mensalidadesParaAlocar]
      .filter(m => m.valorPendente > 0)
      .sort((a, b) => {
        // Priorizar mais antigas
        if (state.configuracaoAlocacao.priorizarMaisAntigos) {
          return (
            new Date(a.mensalidade.vencimento).getTime() -
            new Date(b.mensalidade.vencimento).getTime()
          );
        }
        return 0;
      });

    let valorRestante = valorTotal;
    const saldoDisponivel = state.saldoResponsavel?.saldo || 0;

    if (state.configuracaoAlocacao.considerarSaldo) {
      valorRestante += saldoDisponivel;
    }

    const novasMensalidades = mensalidades.map(m => {
      if (valorRestante <= 0) {
        return { ...m, valorAlocado: 0, selecionada: false };
      }

      const valorAlocar = Math.min(valorRestante, m.valorPendente);
      valorRestante -= valorAlocar;

      return {
        ...m,
        valorAlocado: valorAlocar,
        selecionada: valorAlocar > 0,
      };
    });

    this.patchState({ mensalidadesParaAlocar: novasMensalidades });
  }

  limparAlocacao(): void {
    const mensalidades = this.get().mensalidadesParaAlocar.map(m => ({
      ...m,
      valorAlocado: 0,
      selecionada: false,
    }));

    this.patchState({ mensalidadesParaAlocar: mensalidades });
  }

  obterAlocacoes(): AlocacaoMensalidade[] {
    return this.get()
      .mensalidadesParaAlocar.filter(m => m.valorAlocado > 0)
      .map(m => ({
        mensalidadeId: m.mensalidade.id,
        valor: m.valorAlocado,
      }));
  }
}
