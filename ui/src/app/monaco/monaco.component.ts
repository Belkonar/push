import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import {basicSetup} from "codemirror"
import {EditorView, keymap} from "@codemirror/view"
import { Extension } from "@codemirror/state"
import { javascript } from '@codemirror/lang-javascript';
import {indentWithTab} from "@codemirror/commands"

declare var monaco: any;
/**
 * IMPORTANT: Don't mess with this class, ElementRef is a problem from a sec perspective.
 */
@Component({
  selector: 'app-editor',
  templateUrl: './monaco.component.html',
  styleUrls: ['./monaco.component.scss']
})
export class MonacoComponent implements AfterViewInit, OnChanges {
  id: string = 'editor';

  editor!: EditorView;

  @Input() value: string = '';
  @Output() valueChange = new EventEmitter<string>();

  @Input() lang: 'yaml' | 'rego' = 'rego'

  @ViewChild("editor") editorRef!: ElementRef; // gets #target1

  constructor() { }

  ngAfterViewInit() {
    this.setupEditor()
  }

  // Handle this better, as in not like shit
  ngOnChanges(changes: SimpleChanges) {
    if (this.editor === undefined) {
      return;
    }

    // don't update once we get some content
    if (this.editor.state.doc.length > 0) {
      return;
    }

    this.editor.dispatch({
      changes: {from: 0, insert: changes['value'].currentValue}
    })
  }

  setupEditor() {
    let langMe: Extension[] = [];

    if (this.lang === 'yaml') {
      // langMe = [javascript()]
    }

    if (this.lang === 'rego') {
      // TODO: Rego
    }

    let editor = new EditorView({
      extensions: [basicSetup, keymap.of([indentWithTab]), ...langMe, EditorView.updateListener.of(update => {
        //if (!update.view.hasFocus) {
        if(update.docChanged) {
          this.valueChange.emit(update.state.doc.toString());
        }
        //}
      })],
      parent: this.editorRef.nativeElement,
    })

    this.editor = editor;

    // set the initial data
    editor.dispatch({
      changes: {from: 0, insert: this.value}
    })
  }

}
