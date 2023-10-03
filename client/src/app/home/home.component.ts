import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent {
  registeredMode = false;

  ngOnInit(): void {}

  registerToggle() {
    this.registeredMode = !this.registeredMode;
  }

  cancelRegisterMode(event: boolean) {
    this.registeredMode = event;
  }
}
