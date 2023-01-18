import { Component, OnInit } from '@angular/core';
import {OrgService} from "../../services/org.service";

@Component({
  selector: 'app-org-list',
  templateUrl: './org-list.component.html',
  styleUrls: ['./org-list.component.scss']
})
export class OrgListComponent implements OnInit {
  orgs: any[] = [];

  constructor(private orgService: OrgService) { }

  ngOnInit(): void {
    this.orgService.getOrgs()
      .subscribe(x => this.orgs = x);
  }

  add() {
    let policyName = prompt("Organization Name", "") || "";

    if (policyName.trim() !== "") {
      this.orgService.createOrg(policyName.trim())
        .subscribe(() => {
          this.ngOnInit();
        })
    }
  }
}
