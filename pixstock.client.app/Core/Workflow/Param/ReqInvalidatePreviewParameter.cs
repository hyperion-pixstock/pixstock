using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixstock.client.app.Workflow
{
  public class ReqInvalidatePreviewParameter
  {
    public int Operation;
    public long ContentId;
    public int Position;
  }
}
