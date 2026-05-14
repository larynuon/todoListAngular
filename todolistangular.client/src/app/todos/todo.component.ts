// Main TODO list component
import { Component, OnInit, signal, model } from '@angular/core';
import { TodoApiService, TodoItem } from './todo-api.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-todos',
  standalone: true,  // Modern standalone component (no NgModule needed)
  imports: [CommonModule, FormsModule],  // Import necessary Angular modules
  templateUrl: './todo.component.html',
})
export class TodoComponent implements OnInit {
  // Reactive state using Angular Signals (automatically triggers UI updates)
  items = signal<TodoItem[]>([]);  // List of TODO items
  newTitle = model('');  // Two-way binding for input field
  isLoading = signal(false);  // Loading state for API requests
  error = signal<string | null>(null);  // Error message display

  constructor(private api: TodoApiService) { }

  // Load TODO items when component initializes
  ngOnInit(): void {
    this.refresh();
  }

  // Fetch all TODO items from the API
  refresh(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.api.getAll().subscribe({
      next: (items) => {
        this.items.set(items);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load TODOs.');
        this.isLoading.set(false);
      },
    });
  }

  // Add a new TODO item
  add(): void {
    const title = this.newTitle().trim();
    if (!title) return;  // Don't add empty items

    this.api.create(title).subscribe({
      next: (created) => {
        // Add new item to the beginning of the list
        this.items.set([created, ...this.items()]);
        this.newTitle.set('');  // Clear input field
      },
      error: () => {
        this.error.set('Failed to add TODO.');
      },
    });
  }

  // Delete a TODO item by ID
  remove(id: string): void {
    this.api.delete(id).subscribe({
      next: () => {
        // Filter out the deleted item from the list
        this.items.set(this.items().filter(x => x.id !== id));
      },
      error: () => {
        this.error.set('Failed to delete TODO.');
      },
    });
  }
}
