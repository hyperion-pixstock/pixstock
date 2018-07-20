import { Component } from "@angular/core";

class ExplorerItem {
  constructor(name: string) {
    this.name = name;
  }

  name: string;

  aaa() {
    console.info("click " + this.name);
  }
}

@Component({
  selector: 'explorer-list',
  templateUrl: './explorer-list.fragment.html',
  styleUrls: ['./explorer-list.fragment.scss']
})
export class ExplorerListFragment {
  articles = [
    {
      name: "A",
      aaa() {
        console.info("!!!click " + this.name);
      }
    }
  ];

  constructor() {
    this.articles.push(new ExplorerItem("a"));
    this.articles.push(new ExplorerItem("b"));
  }
}
