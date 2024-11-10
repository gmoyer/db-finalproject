import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-dialog',
  templateUrl: './dialog.component.html',
  styleUrl: './dialog.component.scss'
})
export class DialogComponent {
  @Input() message: string = "";
  @Input() cancelable: boolean = true;
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
