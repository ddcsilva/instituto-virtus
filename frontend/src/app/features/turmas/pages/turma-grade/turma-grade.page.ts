import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';
import { TurmasService } from '../../data-access/turmas.service';

@Component({
  selector: 'app-turma-grade',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatListModule, MatIconModule, PageHeaderComponent],
  template: `
    <app-page-header title="Grade da Turma" subtitle="Visualize a grade horária da turma" />

    <mat-card>
      <mat-card-content>
        <mat-list>
          @for (item of grade(); track item.id) {
          <mat-list-item>
            <mat-icon matListItemIcon>schedule</mat-icon>
            <div matListItemTitle>
              {{ item.diaSemana }} — {{ item.horaInicio }} - {{ item.horaFim }}
            </div>
            <div matListItemLine>Sala: {{ item.sala || '-' }}</div>
          </mat-list-item>
          }
        </mat-list>
      </mat-card-content>
    </mat-card>
  `,
})
export class TurmaGradePage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(TurmasService);

  readonly grade = signal<any[]>([]);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.service.getGradeHorarios(id).subscribe(list => this.grade.set(list || []));
  }
}
