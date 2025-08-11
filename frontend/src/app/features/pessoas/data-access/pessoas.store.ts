import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { tapResponse } from '@ngrx/operators';
import { Observable, switchMap, tap, catchError, EMPTY } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import {
  Pessoa,
  CreatePessoaRequest,
  UpdatePessoaRequest,
  ResponsavelAluno,
  VinculoResponsavelRequest,
  TipoPessoa,
} from '../models/pessoa.model';
import { PessoasService, PessoaFilter, PagedResult } from './pessoas.service';

export interface PessoasState {
  pessoas: Pessoa[];
  selectedPessoa: Pessoa | null;
  vinculos: ResponsavelAluno[];
  loading: boolean;
  error: string | null;
  filter: PessoaFilter;
  total: number;
}

@Injectable()
export class PessoasStore extends ComponentStore<PessoasState> {
  constructor(
    private readonly pessoasService: PessoasService,
    private readonly snackBar: MatSnackBar
  ) {
    super({
      pessoas: [],
      selectedPessoa: null,
      vinculos: [],
      loading: false,
      error: null,
      filter: { page: 0, pageSize: 10 },
      total: 0,
    });
  }

  // Selectors
  readonly pessoas$ = this.select((state) => state.pessoas);
  readonly selectedPessoa$ = this.select((state) => state.selectedPessoa);
  readonly vinculos$ = this.select((state) => state.vinculos);
  readonly loading$ = this.select((state) => state.loading);
  readonly error$ = this.select((state) => state.error);
  readonly filter$ = this.select((state) => state.filter);
  readonly total$ = this.select((state) => state.total);

  readonly alunos$ = this.select(this.pessoas$, (pessoas) =>
    pessoas.filter((p) => p.tipo === 'Aluno')
  );

  readonly responsaveis$ = this.select(this.pessoas$, (pessoas) =>
    pessoas.filter((p) => p.tipo === 'Responsavel')
  );

  readonly professores$ = this.select(this.pessoas$, (pessoas) =>
    pessoas.filter((p) => p.tipo === 'Professor')
  );

  // Updaters
  readonly setLoading = this.updater((state, loading: boolean) => ({
    ...state,
    loading,
  }));

  readonly setError = this.updater((state, error: string | null) => ({
    ...state,
    error,
  }));

  readonly setPessoas = this.updater((state, result: PagedResult<Pessoa>) => ({
    ...state,
    pessoas: result.items,
    total: result.total,
    loading: false,
    error: null,
  }));

  readonly setSelectedPessoa = this.updater((state, pessoa: Pessoa | null) => ({
    ...state,
    selectedPessoa: pessoa,
  }));

  readonly setVinculos = this.updater(
    (state, vinculos: ResponsavelAluno[]) => ({
      ...state,
      vinculos,
    })
  );

  readonly updateFilter = this.updater(
    (state, filter: Partial<PessoaFilter>) => ({
      ...state,
      filter: { ...state.filter, ...filter },
    })
  );

  readonly addPessoa = this.updater((state, pessoa: Pessoa) => ({
    ...state,
    pessoas: [pessoa, ...state.pessoas],
    total: state.total + 1,
  }));

  readonly updatePessoa = this.updater((state, pessoa: Pessoa) => ({
    ...state,
    pessoas: state.pessoas.map((p) => (p.id === pessoa.id ? pessoa : p)),
    selectedPessoa:
      state.selectedPessoa?.id === pessoa.id ? pessoa : state.selectedPessoa,
  }));

  readonly removePessoa = this.updater((state, id: string) => ({
    ...state,
    pessoas: state.pessoas.filter((p) => p.id !== id),
    total: state.total - 1,
  }));

  // Effects
  readonly loadPessoas = this.effect((filter$: Observable<PessoaFilter>) =>
    filter$.pipe(
      tap(() => this.setLoading(true)),
      switchMap((filter) =>
        this.pessoasService.getAll(filter).pipe(
          tapResponse(
            (result) => this.setPessoas(result),
            (error) => {
              this.setError('Erro ao carregar pessoas');
              this.snackBar.open('Erro ao carregar pessoas', 'Fechar', {
                duration: 3000,
              });
            }
          )
        )
      )
    )
  );

  readonly loadPessoa = this.effect((id$: Observable<string>) =>
    id$.pipe(
      tap(() => this.setLoading(true)),
      switchMap((id) =>
        this.pessoasService.getById(id).pipe(
          tapResponse(
            (pessoa) => {
              this.setSelectedPessoa(pessoa);
              this.setLoading(false);
            },
            (error) => {
              this.setError('Erro ao carregar pessoa');
              this.snackBar.open('Erro ao carregar pessoa', 'Fechar', {
                duration: 3000,
              });
            }
          )
        )
      )
    )
  );

  readonly createPessoa = this.effect(
    (pessoa$: Observable<CreatePessoaRequest>) =>
      pessoa$.pipe(
        tap(() => this.setLoading(true)),
        switchMap((pessoa) =>
          this.pessoasService.create(pessoa).pipe(
            tapResponse(
              (created) => {
                this.addPessoa(created);
                this.snackBar.open('Pessoa cadastrada com sucesso', 'Fechar', {
                  duration: 3000,
                });
              },
              (error) => {
                this.setError('Erro ao criar pessoa');
                this.snackBar.open('Erro ao criar pessoa', 'Fechar', {
                  duration: 3000,
                });
              }
            )
          )
        )
      )
  );

  readonly updatePessoaEffect = this.effect(
    (update$: Observable<{ id: string; pessoa: UpdatePessoaRequest }>) =>
      update$.pipe(
        tap(() => this.setLoading(true)),
        switchMap(({ id, pessoa }) =>
          this.pessoasService.update(id, pessoa).pipe(
            tapResponse(
              (updated) => {
                this.updatePessoa(updated);
                this.snackBar.open('Pessoa atualizada com sucesso', 'Fechar', {
                  duration: 3000,
                });
              },
              (error) => {
                this.setError('Erro ao atualizar pessoa');
                this.snackBar.open('Erro ao atualizar pessoa', 'Fechar', {
                  duration: 3000,
                });
              }
            )
          )
        )
      )
  );

  readonly deletePessoa = this.effect((id$: Observable<string>) =>
    id$.pipe(
      tap(() => this.setLoading(true)),
      switchMap((id) =>
        this.pessoasService.delete(id).pipe(
          tapResponse(
            () => {
              this.removePessoa(id);
              this.snackBar.open('Pessoa excluída com sucesso', 'Fechar', {
                duration: 3000,
              });
            },
            (error) => {
              this.setError('Erro ao excluir pessoa');
              this.snackBar.open('Erro ao excluir pessoa', 'Fechar', {
                duration: 3000,
              });
            }
          )
        )
      )
    )
  );

  readonly toggleStatus = this.effect((id$: Observable<string>) =>
    id$.pipe(
      switchMap((id) =>
        this.pessoasService.toggleStatus(id).pipe(
          tapResponse(
            (pessoa) => {
              this.updatePessoa(pessoa);
              const status = pessoa.ativo ? 'ativada' : 'desativada';
              this.snackBar.open(`Pessoa ${status} com sucesso`, 'Fechar', {
                duration: 3000,
              });
            },
            (error) => {
              this.setError('Erro ao alterar status');
              this.snackBar.open('Erro ao alterar status', 'Fechar', {
                duration: 3000,
              });
            }
          )
        )
      )
    )
  );

  readonly loadVinculos = this.effect((responsavelId$: Observable<string>) =>
    responsavelId$.pipe(
      switchMap((responsavelId) =>
        this.pessoasService.getVinculos(responsavelId).pipe(
          tapResponse(
            (vinculos) => this.setVinculos(vinculos),
            (error) => {
              this.setError('Erro ao carregar vínculos');
              this.snackBar.open('Erro ao carregar vínculos', 'Fechar', {
                duration: 3000,
              });
            }
          )
        )
      )
    )
  );

  readonly vincularAlunos = this.effect(
    (request$: Observable<VinculoResponsavelRequest>) =>
      request$.pipe(
        switchMap((request) =>
          this.pessoasService.vincularAlunos(request).pipe(
            tapResponse(
              (vinculos) => {
                this.setVinculos(vinculos);
                this.snackBar.open('Alunos vinculados com sucesso', 'Fechar', {
                  duration: 3000,
                });
              },
              (error) => {
                this.setError('Erro ao vincular alunos');
                this.snackBar.open('Erro ao vincular alunos', 'Fechar', {
                  duration: 3000,
                });
              }
            )
          )
        )
      )
  );
}
