import { group } from '@angular/animations';
import { Component } from '@angular/core';
import { AbstractControl, AbstractControlOptions, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { SecurityService } from 'src/app/shared/services/security.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  signUpForm = new FormGroup({
    username: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(5)
    ]),
    email: new FormControl<string>('', [
      Validators.required,
      Validators.email,
      Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")
    ]),
    password: new FormControl<string>('', [
      Validators.required,
      Validators.pattern("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,}$")
    ]),
    repeatedPassword: new FormControl<string>('', [
      Validators.required
    ])
  });

  constructor(public service: SecurityService) {
    this.signUpForm.setValidators(this.passwordMatchValidator());
  }

  get username() {
    return this.signUpForm.controls.username as FormControl<string>;
  }

  get email() {
    return this.signUpForm.controls.email as FormControl<string>;
  }

  get password() {
    return this.signUpForm.controls.password as FormControl<string>;
  }

  get repeatedPassword() {
    return this.signUpForm.controls.repeatedPassword as FormControl<string>;
  }

  passwordMatchValidator(): ValidatorFn {
    return ((group: AbstractControl): ValidationErrors | null => {
      return group.get('password')?.value === group.get('repeatedPassword')?.value
        ? null : { 'mismatch': true } as ValidationErrors;
    }) as ValidatorFn;
  }

  submit() {
    if (this.signUpForm.valid) {
      this.service.Register({
        email: this.signUpForm.value.email as string,
        username: this.signUpForm.value.username as string,
        password: this.signUpForm.value.password as string
      })
    }
  }
}
