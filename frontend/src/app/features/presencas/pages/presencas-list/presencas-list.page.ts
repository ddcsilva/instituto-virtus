import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';

@Component({
  selector: 'app-presencas-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, RouterLink, PageHeaderComponent],
  template: `
    <app-page-header
      title="Presenças"
      subtitle="Gerencie as presenças dos alunos"
      actionLabel="Lançar Presença"
      actionIcon="fact_check"
      actionRoute="/presencas/lancar"
    />

    <mat-card>
      <mat-card-content>
        <p>Selecione "Lançar Presença" para registrar a presença dos alunos em uma aula.</p>
        <a mat-raised-button color="primary" [routerLink]="['/presencas/lancar']">
          Ir para Lançar Presença
        </a>
      </mat-card-content>
    </mat-card>
  `,
})
export class PresencasListPage {}
