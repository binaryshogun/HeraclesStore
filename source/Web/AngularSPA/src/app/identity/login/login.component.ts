import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { SecurityService } from 'src/app/shared/services/security.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  signInForm = new FormGroup({
    email: new FormControl<string>('', [
      Validators.required,
      Validators.email,
      Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")
    ]),
    password: new FormControl<string>('', [
      Validators.required
    ]),
  });

  constructor(public service: SecurityService) { }

  get email() {
    return this.signInForm.controls.email as FormControl<string>;
  }

  get password() {
    return this.signInForm.controls.password as FormControl<string>;
  }


  submit() {
    if (this.signInForm.valid) {
      this.service.Login({
        email: this.signInForm.value.email as string,
        password: this.signInForm.value.password as string
      })
    }
  }
}
