import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PolicyService } from '../../services/policy.service';

@Component({
  selector: 'app-policy-edit',
  templateUrl: './policy-edit.component.html',
  styleUrls: ['./policy-edit.component.scss']
})
export class PolicyEditComponent implements OnInit {
  editorValue = ''
  key: string = '';

  constructor(private activatedRoute: ActivatedRoute, private policyService: PolicyService) { }

  ngOnInit(): void {
    this.key = this.activatedRoute.snapshot.params['key'];
    this.policyService.getPolicies()
      .subscribe(p => {
        let policy = p.filter(x => x.key === this.key)[0];
        this.editorValue = policy.policy;
      })
  }

  save() {
    this.policyService.updatePolicy(this.key, this.editorValue)
      .subscribe(() => {
        alert('saved')
      })
  }

}
