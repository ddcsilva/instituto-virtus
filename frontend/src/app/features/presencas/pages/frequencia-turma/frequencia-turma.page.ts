import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';

@Component({
  selector: 'app-frequencia-turma',
  standalone: true,
  imports: [CommonModule, MatCardModule, PageHeaderComponent],
  template: `
    <app-page-header
      title="Frequência da Turma"
      subtitle="Visualize a frequência dos alunos da turma"
    />

    <mat-card>
      <mat-card-content>
        <p>Funcionalidade em desenvolvimento...</p>
      </mat-card-content>
    </mat-card>
  `,
})
export class FrequenciaTurmaPage {}
