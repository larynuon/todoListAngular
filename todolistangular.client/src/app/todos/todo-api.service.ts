// Service for making HTTP requests to the TODO API
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// Data model for a TODO item
export interface TodoItem {
  id: string;
  title: string;
  createdAt: string;
}

// Injectable service available throughout the app
@Injectable({ providedIn: 'root' })
export class TodoApiService {
  // Base URL for API endpoints (proxied to backend via proxy.conf.js)
  private readonly baseUrl = '/api/todos';

  constructor(private http: HttpClient) { }

  // Get all TODO items
  getAll(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(this.baseUrl);
  }

  // Create a new TODO item
  create(title: string): Observable<TodoItem> {
    return this.http.post<TodoItem>(this.baseUrl, { title });
  }

  // Delete a TODO item by ID
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
