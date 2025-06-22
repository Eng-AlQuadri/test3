import { Component, inject, OnInit } from '@angular/core';
import { SubjectService } from '../../../../core/services/subject.service';
import { StudentSubjects } from '../../../models/StudentSubjects';

@Component({
  selector: 'app-subjects',
  imports: [],
  templateUrl: './subjects.component.html',
  styleUrl: './subjects.component.css'
})
export class SubjectsComponent implements OnInit{

  private subjectService = inject(SubjectService);

  subjects: StudentSubjects[] = [];

  ngOnInit(): void {
    this.loadSubjects();
  }

  loadSubjects() {
    this.subjectService.getSubjectsForStudent().subscribe({
      next: (response) => this.subjects = response
    })
  }
}
