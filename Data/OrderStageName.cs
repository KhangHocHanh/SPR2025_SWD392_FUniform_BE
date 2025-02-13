using System;
using System.Collections.Generic;

namespace ClothingCustomization.Data;

public partial class OrderStageName
{
    public int OrderStageNameId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<OrderStage> OrderStages { get; set; } = new List<OrderStage>();
}
