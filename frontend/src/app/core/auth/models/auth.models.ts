export interface User {
  id: string;
  nome: string;
  email: string;
  tipo: UserRole;
  pessoaId?: string;
  ativo: boolean;
}

export type UserRole =
  | 'Admin'
  | 'Coordenador'
  | 'Professor'
  | 'Responsavel'
  | 'Aluno';

export interface LoginRequest {
  email: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  user: User;
  expiresIn: number;
}

export interface Session {
  user: User | null;
  token: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
}
