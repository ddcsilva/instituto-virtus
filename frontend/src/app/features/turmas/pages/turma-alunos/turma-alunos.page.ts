import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { TurmasService } from '../../data-access/turmas.service';

@Component({
  selector: 'app-turma-alunos',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatChipsModule,
    MatIconModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header title="Alunos da Turma" subtitle="Gerencie os alunos matriculados na turma" />

    <mat-card>
      <mat-card-content>
        <table mat-table [dataSource]="alunos()" class="full-width">
          <ng-container matColumnDef="nome">
            <th mat-header-cell *matHeaderCellDef>Nome</th>
            <td mat-cell *matCellDef="let a">{{ a.nome }}</td>
          </ng-container>

          <ng-container matColumnDef="cpf">
            <th mat-header-cell *matHeaderCellDef>CPF</th>
            <td mat-cell *matCellDef="let a">{{ a.cpf || '-' }}</td>
          </ng-container>

          <ng-container matColumnDef="telefone">
            <th mat-header-cell *matHeaderCellDef>Telefone</th>
            <td mat-cell *matCellDef="let a">{{ a.telefone }}</td>
          </ng-container>

          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef>Status</th>
            <td mat-cell *matCellDef="let a">
              <mat-chip [ngStyle]="{ background: '#e8f5e9', color: '#2e7d32' }">Ativa</mat-chip>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayed"></tr>
          <tr mat-row *matRowDef="let row; columns: displayed"></tr>
        </table>
      </mat-card-content>
    </mat-card>
  `,
})
export class TurmaAlunosPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(TurmasService);

  readonly alunos = signal<any[]>([]);
  readonly displayed = ['nome', 'cpf', 'telefone', 'status'];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.service.getAlunos(id).subscribe(list => this.alunos.set(list || []));
  }
}
