import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function cpfValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;

    const cpf = control.value.replace(/\D/g, '');

    if (cpf.length !== 11) {
      return { cpf: 'CPF deve ter 11 dígitos' };
    }

    // Validação simplificada - implementar algoritmo completo em produção
    if (/^(\d)\1{10}$/.test(cpf)) {
      return { cpf: 'CPF inválido' };
    }

    return null;
  };
}
