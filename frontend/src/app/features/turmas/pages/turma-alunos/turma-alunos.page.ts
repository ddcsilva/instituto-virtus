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

          <ng-container matColumnDef="idade">
            <th mat-header-cell *matHeaderCellDef>Idade</th>
            <td mat-cell *matCellDef="let a">{{ a.idade }}</td>
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
  readonly displayed = ['nome', 'idade'];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.service.getAlunos(id).subscribe(list => {
      const mapped = (list || []).map((a: any) => ({
        id: a.Id ?? a.id,
        nome: a.Nome ?? a.nome,
        idade: a.Idade ?? a.idade,
      }));
      this.alunos.set(mapped);
    });
  }
}
