import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-confirm',
  templateUrl: './confirm.component.html',
  styleUrl: './confirm.component.scss'
})
export class ConfirmComponent {
  @Input() message: string = "";
  @Output() confirm = new EventEmitter();
  @Output() cancel = new EventEmitter();

  constructor() { }

  onYes() {
    this.confirm.emit();
  }

  onNo() {
    this.cancel.emit();
  }
}
