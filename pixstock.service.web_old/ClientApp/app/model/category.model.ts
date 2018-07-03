import { Label } from "./label.model";

export interface Category {
  Id: number;
  Name: string;
  HasLinkSubCategoryFlag: boolean;
  Labels: Array<Label>;
}
