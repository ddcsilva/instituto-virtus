import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

import { PageHeaderComponent } from '../../../../shared/ui/components/page-header/page-header.component';

interface RelatorioCard {
  titulo: string;
  descricao: string;
  icone: string;
  cor: string;
  rota: string;
}

@Component({
  selector: 'app-relatorios-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    PageHeaderComponent,
  ],
  template: `
    <app-page-header
      title="Relatórios"
      subtitle="Acesse relatórios gerenciais e acadêmicos"
    />

    <div class="relatorios-grid">
      @for (relatorio of relatorios; track relatorio.rota) {
      <mat-card class="relatorio-card" [routerLink]="relatorio.rota">
        <mat-card-content>
          <mat-icon [style.color]="relatorio.cor">{{
            relatorio.icone
          }}</mat-icon>
          <h3>{{ relatorio.titulo }}</h3>
          <p>{{ relatorio.descricao }}</p>
        </mat-card-content>
      </mat-card>
      }
    </div>
  `,
  styles: [
    `
      .relatorios-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
        gap: 24px;
      }

      .relatorio-card {
        cursor: pointer;
        transition: transform 0.2s, box-shadow 0.2s;

        &:hover {
          transform: translateY(-4px);
          box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
        }

        mat-card-content {
          padding: 32px;
          text-align: center;
        }

        mat-icon {
          font-size: 64px;
          width: 64px;
          height: 64px;
          margin: 0 auto 16px;
        }

        h3 {
          margin: 16px 0 8px;
          font-size: 20px;
        }

        p {
          color: #666;
          margin: 0;
        }
      }
    `,
  ],
})
export class RelatoriosDashboardPage {
  readonly relatorios: RelatorioCard[] = [
    {
      titulo: 'Inadimplentes',
      descricao: 'Lista de alunos com mensalidades em atraso',
      icone: 'warning',
      cor: '#f44336',
      rota: '/relatorios/inadimplentes',
    },
    {
      titulo: 'Frequência por Turma',
      descricao: 'Relatório de presença e ausência dos alunos',
      icone: 'fact_check',
      cor: '#2196f3',
      rota: '/relatorios/frequencia',
    },
    {
      titulo: 'Aprovados e Reprovados',
      descricao: 'Situação final dos alunos por turma',
      icone: 'school',
      cor: '#4caf50',
      rota: '/relatorios/aprovacao',
    },
  ];
}
