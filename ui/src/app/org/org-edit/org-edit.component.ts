import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {KV} from "../../models";

@Component({
  selector: 'app-org-edit',
  templateUrl: './org-edit.component.html',
  styleUrls: ['./org-edit.component.scss']
})
export class OrgEditComponent implements OnInit {
  activeTab: 'general' | 'policy' = 'general'

  id: string = '';

  metadata: KV[] = [];
  privateMetadata: KV[] = [];

  constructor(private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.id = this.activatedRoute.snapshot.params['id'];
    console.log(this.id)
  }

  addRow(which: 'metadata' | 'private') {
    if (which === 'metadata') {
      this.metadata = [
        ...this.metadata,
        {
          key: '',
          value: ''
        }
      ]
    }
    else {
      this.privateMetadata = [
        ...this.privateMetadata,
        {
          key: '',
          value: ''
        }
      ]
    }
  }

  removeRow(which: 'metadata' | 'private', key: string) {
    if (which === 'metadata') {
      this.metadata = this.metadata.filter(x => x.key !== key);
    }
    else {
      this.privateMetadata = this.privateMetadata.filter(x => x.key !== key);
    }
  }
}
