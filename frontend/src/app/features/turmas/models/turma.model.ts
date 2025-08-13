import { Curso } from '../../cursos/models/curso.model';

export interface Turma {
  id: string;
  cursoId: string;
  nome: string;
  cursoNome?: string;
  professorId: string;
  professorNome?: string;
  // Campos de exibição herdados do domínio
  capacidade?: number;
  sala?: string;
  anoLetivo?: number;
  periodo?: number; // 1 ou 2
  periodoInicio: string;
  periodoFim: string;
  vagas: number;
  vagasOcupadas: number;
  turno: Turno;
  status: StatusTurma;
  horarios: HorarioAula[];
  curso?: Curso;
  professor?: {
    id: string;
    nome: string;
  };
}

export type Turno = 'Manha' | 'Tarde' | 'Noite';
export type StatusTurma = 'Planejada' | 'EmAndamento' | 'Concluida' | 'Cancelada';
export type DiaSemana = 'Segunda' | 'Terca' | 'Quarta' | 'Quinta' | 'Sexta' | 'Sabado' | 'Domingo';

export interface HorarioAula {
  id?: string;
  diaSemana: DiaSemana;
  horaInicio: string; // HH:mm
  horaFim: string; // HH:mm (sempre 50 minutos após início)
  sala?: string;
}

export interface CreateTurmaRequest {
  cursoId: string;
  nome: string;
  professorId: string;
  periodoInicio: string;
  periodoFim: string;
  vagas: number;
  turno: Turno;
  horarios: HorarioAula[];
}

export interface UpdateTurmaRequest extends CreateTurmaRequest {
  status: StatusTurma;
}
