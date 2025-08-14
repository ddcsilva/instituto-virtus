import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBar } from '@angular/material/snack-bar';

import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { PessoasService } from '../../../pessoas/data-access/pessoas.service';

@Component({
  selector: 'app-usuarios-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatChipsModule,
    MatMenuModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header
      title="Usuários"
      subtitle="Gestão de acesso ao sistema"
      actionLabel="Novo Usuário"
      actionIcon="person_add"
      actionRoute="/usuarios/novo"
    />

    <mat-card>
      <mat-card-content>
        <div class="filters">
          <mat-form-field>
            <mat-label>Buscar</mat-label>
            <input matInput [formControl]="searchControl" placeholder="Nome ou e-mail" />
            <mat-icon matSuffix>search</mat-icon>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Perfil</mat-label>
            <mat-select [formControl]="perfilControl">
              <mat-option value="">Todos</mat-option>
              <mat-option value="Coordenador">Coordenador</mat-option>
              <mat-option value="Professor">Professor</mat-option>
              <mat-option value="Responsavel">Responsável</mat-option>
              <mat-option value="Aluno">Aluno</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Status</mat-label>
            <mat-select [formControl]="statusControl">
              <mat-option [value]="null">Todos</mat-option>
              <mat-option [value]="true">Ativo</mat-option>
              <mat-option [value]="false">Inativo</mat-option>
            </mat-select>
          </mat-form-field>
        </div>

        <table mat-table [dataSource]="usuarios()" class="full-width">
          <ng-container matColumnDef="nome">
            <th mat-header-cell *matHeaderCellDef>Nome</th>
            <td mat-cell *matCellDef="let u">{{ u.nome }}</td>
          </ng-container>
          <ng-container matColumnDef="email">
            <th mat-header-cell *matHeaderCellDef>E-mail</th>
            <td mat-cell *matCellDef="let u">{{ u.email || '-' }}</td>
          </ng-container>
          <ng-container matColumnDef="perfil">
            <th mat-header-cell *matHeaderCellDef>Perfil</th>
            <td mat-cell *matCellDef="let u">
              <mat-chip>{{ u.tipo }}</mat-chip>
            </td>
          </ng-container>
          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef>Status</th>
            <td mat-cell *matCellDef="let u">
              <mat-chip color="primary" *ngIf="u.ativo; else inativo">Ativo</mat-chip>
              <ng-template #inativo><mat-chip color="warn">Inativo</mat-chip></ng-template>
            </td>
          </ng-container>
          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef>Ações</th>
            <td mat-cell *matCellDef="let u">
              <button mat-icon-button [matMenuTriggerFor]="menu">
                <mat-icon>more_vert</mat-icon>
              </button>
              <mat-menu #menu="matMenu">
                <a mat-menu-item [routerLink]="['/pessoas', u.id, 'editar']">
                  <mat-icon>person</mat-icon>
                  <span>Ver pessoa</span>
                </a>
                <button mat-menu-item (click)="resetSenha(u)">
                  <mat-icon>lock_reset</mat-icon>
                  <span>Resetar senha</span>
                </button>
              </mat-menu>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
        </table>
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `
      .filters {
        display: flex;
        gap: 16px;
        margin-bottom: 16px;
      }
      table {
        width: 100%;
      }
    `,
  ],
})
export class UsuariosListPage implements OnInit {
  private readonly pessoasService = inject(PessoasService);
  private readonly snackBar = inject(MatSnackBar);

  readonly usuarios = signal<any[]>([]);
  readonly displayedColumns = ['nome', 'email', 'perfil', 'status', 'acoes'];

  readonly searchControl = new FormControl('');
  readonly perfilControl = new FormControl('');
  readonly statusControl = new FormControl<boolean | null>(null);

  ngOnInit(): void {
    this.loadUsuarios();
    this.searchControl.valueChanges.subscribe(() => this.loadUsuarios());
    this.perfilControl.valueChanges.subscribe(() => this.loadUsuarios());
    this.statusControl.valueChanges.subscribe(() => this.loadUsuarios());
  }

  loadUsuarios(): void {
    const params: any = {
      nome: this.searchControl.value || undefined,
      tipo: this.perfilControl.value || undefined,
      ativo: this.statusControl.value ?? undefined,
      somenteUsuarios: true,
      page: 0,
      pageSize: 50,
    };
    this.pessoasService.getAll(params).subscribe(result => {
      this.usuarios.set(result.items);
    });
  }

  resetSenha(u: any): void {
    const novaSenha = prompt(`Nova senha para ${u.nome}`);
    if (!novaSenha || novaSenha.length < 6) return;
    fetch('/api/auth/set-password', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ pessoaId: u.id, novaSenha }),
    })
      .then(() => this.snackBar.open('Senha atualizada', 'Fechar', { duration: 3000 }))
      .catch(() => this.snackBar.open('Erro ao atualizar senha', 'Fechar', { duration: 3000 }));
  }
}
