import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';

@Component({
  selector: 'app-turma-grade',
  standalone: true,
  imports: [CommonModule, MatCardModule, PageHeaderComponent],
  template: `
    <app-page-header
      title="Grade da Turma"
      subtitle="Visualize a grade horária da turma"
    />

    <mat-card>
      <mat-card-content>
        <p>Funcionalidade em desenvolvimento...</p>
      </mat-card-content>
    </mat-card>
  `,
})
export class TurmaGradePage {}
