import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NgxMaskDirective, provideNgxMask } from 'ngx-mask';
import { map, catchError, debounceTime, of } from 'rxjs';

import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { cpfValidator } from '../../../../shared/validators/cpf.validator';
import { telefoneValidator } from '../../../../shared/validators/telefone.validator';
import { PessoasStore } from '../../data-access/pessoas.store';
import { PessoasService } from '../../data-access/pessoas.service';
import { CreatePessoaRequest, UpdatePessoaRequest, TipoPessoa } from '../../models/pessoa.model';

@Component({
  selector: 'app-pessoa-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    NgxMaskDirective,
    PageHeaderComponent,
  ],
  providers: [PessoasStore, provideNgxMask()],
  template: `
    <app-page-header
      [title]="isEdit() ? 'Editar Pessoa' : 'Nova Pessoa'"
      [subtitle]="isEdit() ? 'Atualize os dados da pessoa' : 'Cadastre uma nova pessoa'"
    />

    <mat-card>
      <mat-card-content>
        @if (loading()) {
        <div class="loading">
          <mat-spinner />
        </div>
        } @else {
        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <div class="form-grid">
            <mat-form-field>
              <mat-label>Nome Completo</mat-label>
              <input matInput formControlName="nome" placeholder="Digite o nome completo" />
              @if (form.get('nome')?.hasError('required') && form.get('nome')?.touched) {
              <mat-error>Nome é obrigatório</mat-error>
              }
            </mat-form-field>

            <mat-form-field>
              <mat-label>CPF</mat-label>
              <input
                matInput
                formControlName="cpf"
                mask="000.000.000-00"
                placeholder="000.000.000-00"
              />
              @if (form.get('cpf')?.hasError('required') && form.get('cpf')?.touched) {
              <mat-error>CPF é obrigatório</mat-error>
              } @if (form.get('cpf')?.hasError('cpf') && form.get('cpf')?.touched) {
              <mat-error>CPF inválido</mat-error>
              } @if (form.get('cpf')?.hasError('cpfJaCadastrado') && form.get('cpf')?.touched) {
              <mat-error>CPF já cadastrado</mat-error>
              }
            </mat-form-field>

            <mat-form-field>
              <mat-label>Data de Nascimento</mat-label>
              <input
                matInput
                formControlName="dataNascimento"
                placeholder="dd/mm/aaaa"
                mask="00/00/0000"
              />
              @if (form.get('dataNascimento')?.hasError('required') &&
              form.get('dataNascimento')?.touched) {
              <mat-error>Data de nascimento é obrigatória</mat-error>
              }
            </mat-form-field>

            <mat-form-field>
              <mat-label>Telefone</mat-label>
              <input
                matInput
                formControlName="telefone"
                mask="(00) 00000-0000"
                placeholder="(00) 00000-0000"
              />
              @if (form.get('telefone')?.hasError('required') && form.get('telefone')?.touched) {
              <mat-error>Telefone é obrigatório</mat-error>
              } @if (form.get('telefone')?.hasError('telefone') && form.get('telefone')?.touched) {
              <mat-error>Telefone inválido</mat-error>
              }
            </mat-form-field>

            <mat-form-field>
              <mat-label>E-mail</mat-label>
              <input
                matInput
                type="email"
                formControlName="email"
                placeholder="email@exemplo.com"
              />
              @if (form.get('email')?.hasError('email') && form.get('email')?.touched) {
              <mat-error>E-mail inválido</mat-error>
              }
              <mat-hint>Opcional para alunos menores de 18 anos</mat-hint>
            </mat-form-field>

            <mat-form-field>
              <mat-label>Tipo</mat-label>
              <mat-select formControlName="tipo">
                <mat-option value="Aluno">Aluno</mat-option>
                <mat-option value="Responsavel">Responsável</mat-option>
                <mat-option value="Professor">Professor</mat-option>
              </mat-select>
              @if (form.get('tipo')?.hasError('required') && form.get('tipo')?.touched) {
              <mat-error>Tipo é obrigatório</mat-error>
              }
            </mat-form-field>

            @if (isEdit()) {
            <mat-form-field>
              <mat-label>Status</mat-label>
              <mat-select formControlName="ativo">
                <mat-option [value]="true">Ativo</mat-option>
                <mat-option [value]="false">Inativo</mat-option>
              </mat-select>
            </mat-form-field>
            }
          </div>

          <div class="form-actions">
            <button mat-button type="button" (click)="cancel()">Cancelar</button>
            <button
              mat-raised-button
              color="primary"
              type="submit"
              [disabled]="form.invalid || loading()"
            >
              {{ isEdit() ? 'Atualizar' : 'Cadastrar' }}
            </button>
          </div>
        </form>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `
      .loading {
        display: flex;
        justify-content: center;
        padding: 48px;
      }

      .form-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
        gap: 16px;
        margin-bottom: 24px;
      }

      .form-actions {
        display: flex;
        justify-content: flex-end;
        gap: 8px;
        padding-top: 16px;
        border-top: 1px solid #e0e0e0;
      }
    `,
  ],
})
export class PessoaFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly store = inject(PessoasStore);
  private readonly pessoasService = inject(PessoasService);

  readonly isEdit = signal(false);
  readonly loading = signal(false);
  readonly pessoaId = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group(
    {
      nome: ['', Validators.required],
      cpf: ['', [Validators.required, cpfValidator()], [this.cpfUnicoAsyncValidator()]],
      dataNascimento: ['', Validators.required],
      telefone: ['', [Validators.required, telefoneValidator()]],
      email: ['', Validators.email],
      tipo: ['Aluno' as TipoPessoa, Validators.required],
      ativo: [true],
    },
    { updateOn: 'blur' }
  );

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit.set(true);
      this.pessoaId.set(id);
      this.loadPessoa(id);
    }

    // Auto-validate email for adults
    this.form.get('dataNascimento')?.valueChanges.subscribe(date => {
      if (date) {
        const age = this.calculateAge(new Date(date));
        const emailControl = this.form.get('email');

        if (age >= 18 && this.form.get('tipo')?.value === 'Aluno') {
          emailControl?.setValidators([Validators.required, Validators.email]);
        } else {
          emailControl?.setValidators([Validators.email]);
        }
        emailControl?.updateValueAndValidity();
      }
    });
  }

  loadPessoa(id: string): void {
    this.loading.set(true);
    this.store.loadPessoa(id);

    this.store.selectedPessoa$.subscribe(pessoa => {
      if (pessoa) {
        this.form.patchValue({
          nome: pessoa.nome,
          cpf: pessoa.cpf,
          dataNascimento: pessoa.dataNascimento,
          telefone: pessoa.telefone,
          email: pessoa.email || '',
          tipo: pessoa.tipo,
          ativo: pessoa.ativo,
        });
        this.loading.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    const formValue = this.form.getRawValue();

    if (this.isEdit() && this.pessoaId()) {
      const updateRequest: UpdatePessoaRequest = {
        ...formValue,
        email: formValue.email || undefined,
      };

      this.store.updatePessoaEffect({
        id: this.pessoaId()!,
        pessoa: updateRequest,
      });
    } else {
      const createRequest: CreatePessoaRequest = {
        ...formValue,
        email: formValue.email || undefined,
      };

      this.store.createPessoa(createRequest);
    }

    // Navega ao concluir
    const sub = this.store.loading$.subscribe(loading => {
      if (!loading) {
        sub.unsubscribe();
        this.router.navigate(['/pessoas']);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/pessoas']);
  }

  private calculateAge(birthDate: Date): number {
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();

    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }

    return age;
  }

  private cpfUnicoAsyncValidator() {
    return (control: any) => {
      const value: string = control?.value;
      if (!value) return of(null);
      // normalizar cpf
      const digits = value.replace(/\D/g, '');
      if (digits.length !== 11) return of(null);

      return this.pessoasService.getAll({ cpf: digits, page: 0, pageSize: 1 }).pipe(
        map(res => {
          const found = res.items?.[0];
          if (!found) return null;
          // Se edição e o mesmo registro, válido
          if (this.isEdit() && this.pessoaId() === found.id) return null;
          return { cpfJaCadastrado: true };
        }),
        catchError(() => of(null))
      );
    };
  }
}
