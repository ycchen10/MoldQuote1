using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MoldQuote.DAL
{
    /// <summary>
    /// 创建圆形面工厂
    /// </summary>
    public class CylinderBodyFactory
    {
        public static AbstractCylinderBody Create(StepBuilder builder)
        {
            int count = builder.CylFeater.Count;
            AbstractCylinderBody abs = null;
            switch (count)
            {
                case 1:
                    abs = new CylinderBody(builder);
                    break;
                case 2:
                    abs = new CylinderTwoStepBody(builder);
                    break;
                default:
                    abs = new CylinderManyStepBody(builder);
                    break;
            }
            return abs;
        }
    }
}
