import { Component, OnInit, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuthService } from '../../core/auth/services/auth.service';
import { PageHeaderComponent } from '../../shared/ui/components/page-header/page-header.component';

interface DashboardCard {
  title: string;
  value: string | number;
  icon: string;
  color: string;
  route: string;
  trend?: number;
}

interface QuickAction {
  label: string;
  icon: string;
  route: string;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    PageHeaderComponent,
  ],
  templateUrl: './dashboard.page.html',
  styleUrls: ['./dashboard.page.scss'],
})
export class DashboardPage implements OnInit {
  private readonly authService = inject(AuthService);

  readonly nomeUsuario = signal('');
  readonly tipoUsuario = signal('');
  readonly carregando = signal(true);

  constructor() {
    // Reage às mudanças no usuário autenticado
    effect(
      () => {
        const usuario = this.authService.usuarioAtual();
        const estaAutenticado = this.authService.estaAutenticado();

        if (estaAutenticado && usuario && usuario.nome) {
          this.nomeUsuario.set(usuario.nome);
          this.tipoUsuario.set(this.obterNomeDoTipo(usuario.tipo));
          this.loadDashboardData(usuario.tipo);
          this.carregando.set(false);
        }
      },
      { allowSignalWrites: true }
    );
  }

  readonly cards = signal<DashboardCard[]>([]);
  readonly actions = signal<QuickAction[]>([]);
  readonly atividadesRecentes = signal<any[]>([]);
  readonly minhasTurmas = signal<any[]>([]);
  readonly proximosPagamentos = signal<any[]>([]);

  ngOnInit(): void {
    // O effect no constructor já cuida do carregamento dos dados
  }

  obterNomeDoTipo(tipo: string): string {
    const roles: Record<string, string> = {
      Admin: 'Administrador',
      Coordenador: 'Coordenador',
      Professor: 'Professor',
      Responsavel: 'Responsável',
      Aluno: 'Aluno',
    };
    return roles[tipo] || tipo;
  }

  ehAdministrador(): boolean {
    return this.authService.usuarioAtual()?.tipo === 'Admin';
  }

  ehCoordenador(): boolean {
    return this.authService.usuarioAtual()?.tipo === 'Coordenador';
  }

  ehProfessor(): boolean {
    return this.authService.usuarioAtual()?.tipo === 'Professor';
  }

  ehAluno(): boolean {
    return this.authService.usuarioAtual()?.tipo === 'Aluno';
  }

  ehResponsavel(): boolean {
    return this.authService.usuarioAtual()?.tipo === 'Responsavel';
  }

  cardsVisiveis(): DashboardCard[] {
    const userType = this.authService.usuarioAtual()?.tipo;

    if (userType === 'Admin' || userType === 'Coordenador') {
      return [
        {
          title: 'Total de Alunos',
          value: 245,
          icon: 'school',
          color: '#4caf50',
          route: '/pessoas',
          trend: 5,
        },
        {
          title: 'Turmas Ativas',
          value: 18,
          icon: 'groups',
          color: '#2196f3',
          route: '/turmas',
        },
        {
          title: 'Mensalidades em Aberto',
          value: 'R$ 15.480',
          icon: 'attach_money',
          color: '#ff9800',
          route: '/financeiro',
        },
        {
          title: 'Taxa de Presença',
          value: '92%',
          icon: 'fact_check',
          color: '#9c27b0',
          route: '/presencas',
          trend: -2,
        },
      ];
    }

    if (userType === 'Professor') {
      return [
        {
          title: 'Minhas Turmas',
          value: 4,
          icon: 'groups',
          color: '#2196f3',
          route: '/turmas',
        },
        {
          title: 'Total de Alunos',
          value: 68,
          icon: 'school',
          color: '#4caf50',
          route: '/turmas',
        },
        {
          title: 'Aulas Hoje',
          value: 3,
          icon: 'event',
          color: '#ff9800',
          route: '/presencas',
        },
        {
          title: 'Avaliações Pendentes',
          value: 12,
          icon: 'grade',
          color: '#9c27b0',
          route: '/avaliacoes',
        },
      ];
    }

    return [];
  }

  acoesVisiveis(): QuickAction[] {
    const userType = this.authService.usuarioAtual()?.tipo;

    if (userType === 'Admin' || userType === 'Coordenador') {
      return [
        {
          label: 'Nova Matrícula',
          icon: 'add_circle',
          route: '/matriculas/nova',
          color: '#4caf50',
        },
        {
          label: 'Registrar Pagamento',
          icon: 'payment',
          route: '/financeiro/conciliacao',
          color: '#2196f3',
        },
        {
          label: 'Lançar Presença',
          icon: 'fact_check',
          route: '/presencas',
          color: '#ff9800',
        },
        {
          label: 'Gerar Relatório',
          icon: 'assessment',
          route: '/relatorios',
          color: '#9c27b0',
        },
      ];
    }

    if (userType === 'Professor') {
      return [
        {
          label: 'Lançar Presença',
          icon: 'fact_check',
          route: '/presencas',
          color: '#ff9800',
        },
        {
          label: 'Lançar Notas',
          icon: 'grade',
          route: '/avaliacoes',
          color: '#9c27b0',
        },
        {
          label: 'Minhas Turmas',
          icon: 'groups',
          route: '/turmas',
          color: '#2196f3',
        },
      ];
    }

    if (userType === 'Aluno' || userType === 'Responsavel') {
      return [
        {
          label: 'Minhas Turmas',
          icon: 'groups',
          route: '/portal/turmas',
          color: '#2196f3',
        },
        {
          label: 'Financeiro',
          icon: 'attach_money',
          route: '/portal/financeiro',
          color: '#4caf50',
        },
        {
          label: 'Boletim',
          icon: 'grade',
          route: '/portal/boletim',
          color: '#9c27b0',
        },
        {
          label: 'Frequência',
          icon: 'fact_check',
          route: '/portal/frequencia',
          color: '#ff9800',
        },
      ];
    }

    return [];
  }

  loadDashboardData(userType: string): void {
    // Simulated data - replace with actual API calls
    if (userType === 'Admin' || userType === 'Coordenador') {
      this.atividadesRecentes.set([
        {
          id: 1,
          description: 'Nova matrícula: João Silva - Violão Básico',
          timestamp: new Date(),
          icon: 'person_add',
          color: '#4caf50',
        },
        {
          id: 2,
          description: 'Pagamento recebido: Maria Santos - R$ 250,00',
          timestamp: new Date(),
          icon: 'payment',
          color: '#2196f3',
        },
        {
          id: 3,
          description: 'Turma criada: Teologia I - Manhã',
          timestamp: new Date(),
          icon: 'group_add',
          color: '#ff9800',
        },
      ]);
    }

    if (userType === 'Aluno' || userType === 'Responsavel') {
      this.minhasTurmas.set([
        { id: 1, nome: 'Violão Básico', horario: 'Seg/Qua 14:00' },
        { id: 2, nome: 'Teologia I', horario: 'Ter/Qui 19:00' },
      ]);

      this.proximosPagamentos.set([
        { id: 1, competencia: 'Janeiro/2024', valor: 250, vencido: false },
        { id: 2, competencia: 'Fevereiro/2024', valor: 250, vencido: false },
      ]);
    }

    this.carregando.set(false);
  }
}
