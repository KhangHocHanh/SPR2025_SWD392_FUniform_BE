using System;
using System.Collections.Generic;

namespace ClothingCustomization.Data;

public partial class OrderStage
{
    public int OrderStageId { get; set; }

    public int OrderId { get; set; }

    public int OrderStageNameId { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual OrderStageName OrderStageName { get; set; } = null!;
}
