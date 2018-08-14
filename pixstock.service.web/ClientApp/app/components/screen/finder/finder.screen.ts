import { Component } from "@angular/core";
import { ContentListPageItem } from "../../../viewmodel";
import { DeliveryService } from "../../../service/delivery.service";
import { ItemListSelectEventArg } from "ClientApp/app/event";

@Component({
  selector: 'finder',
  templateUrl: './finder.screen.html',
  styleUrls: ['./finder.screen.scss']
})
export class FinderScreen {
  private LOGEVENT: string = "[Pixstock][FinderScreen]";


  constructor(
    private delivery: DeliveryService) {

  }

  onItemSelect(args: ItemListSelectEventArg) {
    console.debug(this.LOGEVENT + "[onItemSelect] - IN");
    console.debug(this.LOGEVENT + "[onItemSelect] 項目選択", args);

    let item = args.item as ContentListPageItem;
    this.delivery.showScreenPreview(args.position);

    console.debug(this.LOGEVENT + "[onItemSelect] - OUT");
  }
}
