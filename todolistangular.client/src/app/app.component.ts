import { Component } from '@angular/core';
import { TodoComponent } from './todos/todo.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [TodoComponent],
  templateUrl: './app.component.html',
})
export class AppComponent { }
