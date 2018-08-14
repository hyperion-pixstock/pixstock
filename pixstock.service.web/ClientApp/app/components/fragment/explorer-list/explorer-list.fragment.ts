import { Component, EventEmitter, Output, AfterViewChecked, AfterViewInit } from "@angular/core";
import { ViewModel, ContentListPageItem } from "../../../viewmodel";
import { ItemListSelectEventArg } from "ClientApp/app/event";

@Component({
  selector: 'explorer-list',
  templateUrl: './explorer-list.fragment.html',
  styleUrls: ['./explorer-list.fragment.scss']
})
export class ExplorerListFragment implements AfterViewInit  {
  private LOGEVENT: string = "[Pixstock][ExplorerListFragment]";

  @Output("item-select")
  private itemSelectedMessage = new EventEmitter();

  /**
   * コンストラクタ
   * @param viewmodel ViewModel
   */
  constructor(private viewmodel: ViewModel) {

  }

  ngAfterViewInit() {

  }

  onClick(item: ContentListPageItem, position: number) {
    console.debug(this.LOGEVENT + "[onClick] - IN", item, "選択位置", position);
    this.itemSelectedMessage.next(new ItemListSelectEventArg(this, item, position));
    console.debug(this.LOGEVENT + "[onClick] - OUT");
  }
}
