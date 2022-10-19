import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MonacoService } from '../monaco.service';
import { filter } from 'rxjs/operators';

declare var monaco: any;
/**
 * IMPORTANT: Don't mess with this class, ElementRef is a problem from a sec perspective.
 * Only use monaco from this component.
 */
@Component({
  selector: 'app-monaco',
  templateUrl: './monaco.component.html',
  styleUrls: ['./monaco.component.scss']
})
export class MonacoComponent implements OnInit {
  id: string = 'editor'

  @Input() value: string = '';
  @Output() valueChange = new EventEmitter<string>();

  @Input() lang: 'javascript' | 'rego' = 'javascript'

  @ViewChild("editor") editorRef!: ElementRef; // gets #target1

  constructor(private monacoService: MonacoService) { }

  ngOnInit(): void {
    this.monacoService.load();
    this.monacoService.loaded
      .pipe(filter(x => x === true))
      .subscribe(() => this.initMonaco())
  }

  initMonaco() {
    const editor = monaco.editor.create(this.editorRef.nativeElement, {
      value: this.value,
      language: this.lang,
      minimap: {
        enabled: false
      }
    });

    editor.onDidChangeModelContent(() => {
      this.valueChange.emit(editor.getValue());
    })
  }

}
