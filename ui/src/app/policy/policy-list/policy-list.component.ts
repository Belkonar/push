import { Component, OnInit } from '@angular/core';
import { PolicyService } from '../../services/policy.service';

@Component({
  selector: 'app-policy-list',
  templateUrl: './policy-list.component.html',
  styleUrls: ['./policy-list.component.scss']
})
export class PolicyListComponent implements OnInit {

  data: any[] = [];

  constructor(private policyService: PolicyService) { }

  ngOnInit(): void {
    this.policyService.getPolicies()
      .subscribe(x => this.data = x);
  }

  add() {
    let policyName = prompt("Policy Name", "") || "";

    if (policyName.trim() !== "") {
      this.policyService.createPolicy(policyName.trim())
        .subscribe(() => {
          this.ngOnInit();
        })
    }
  }
}
