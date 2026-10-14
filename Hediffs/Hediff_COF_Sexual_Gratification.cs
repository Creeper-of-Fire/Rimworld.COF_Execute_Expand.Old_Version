using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COF_Torture.Body;
using COF_Torture.Data;
using COF_Torture.Utility;
using COF_Torture.Utility.DefOf;
using Verse;
using Verse.Noise;

namespace COF_Torture.Hediffs
{
    /// <summary>
    /// 通过这个类的Severity来标识快感，并且只通过这个类内置的方法来改变快感
    /// </summary>
    public class Hediff_COF_Sexual_Gratification : Hediff
    {
        public const float orgasmSeverity = 1;
        private const bool canMultiOrgasm = true;
        private float totalGratification = 0;
        public float TotalGratification => totalGratification;

        private void IsOrgasm(Sexual_Gratification OrgasmReason)
        {
            if (orgasmSeverity < this.totalGratification)
            {
                int OrgasmTimes = 1;
                if (canMultiOrgasm)
                {
                    OrgasmTimes = (int)(totalGratification / orgasmSeverity);
                    var severityLeft = totalGratification - OrgasmTimes * orgasmSeverity;
                    totalGratification = severityLeft;
                    //如果剩下的高潮指示条还能允许一次高潮，就不重置它
                }

                TortureUtility.Orgasm(pawn, OrgasmTimes, OrgasmReason);
                totalGratification = 0.01f; //TODO "残留快感"系统还没做，且这个连续高潮系统是暂时的，之后会继续修改
            }
        }

        public void Set_Gratification(Sexual_Gratification value)
        {
            value.amount -= totalGratification;
            InOrDecrease_Gratification(value);
        }
        
        public void InOrDecrease_Gratification(Sexual_Gratification increment)
        {
            if (increment.amount >= 0)
            {
                Increase_Gratification(increment);
            }
            else
            {
                Decrease_Gratification(increment);
            }
        }

        private void Increase_Gratification(Sexual_Gratification increment)
        {
            totalGratification += increment.amount;
            IsOrgasm(increment);
        }

        private void Decrease_Gratification(Sexual_Gratification increment)
        {
            totalGratification += increment.amount;
            //IsOrgasm(sexualGratification);
        }

        public static Hediff_COF_Sexual_Gratification GetHediffOrgasmIndicator(Pawn pawn)
        {
            var h = (Hediff_COF_Sexual_Gratification)pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf
                .COF_Torture_OrgasmIndicator);
            if (h == null)
            {
                h = (Hediff_COF_Sexual_Gratification)HediffMaker.MakeHediff(HediffDefOf.COF_Torture_OrgasmIndicator,
                    pawn);
                pawn.health.AddHediff(h);
            }

            if (h == null)
            {
                ModLog.Error("配置错误:xml中COF_Torture_OrgasmIndicator不存在或不是Hediff_COF_Sexual_Gratification");
            }

            return h;
        }
    }

    public class Sexual_Gratification
    {
        public float amount;
        public string reason; //TODO 之后可能不是string类型，之后再说

        public Sexual_Gratification(float amount = 0, string reason = "")
        {
            this.amount = amount;
            this.reason = reason;
        }
    }
}