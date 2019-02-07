using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class Costs
    {
        // Энергопотребление, кВт/ч
        public float EnergyConsumption_kWtByHour;

        // Электроэнергия: евро в час
        public float ElectricalCost_EuroPerHour;

        // Стоимость рабоыт персонала: евро в час
        public float StaffCosts_EuroPerHour;

        // Повышающий коэффициент.
        public float BoostFactor;

        // Стоимость гранулята, евро/кг
        public float GranulateCosts_EuroPerKg;

        // Стоимость работы оборудования в минуту.
        public float EquipmentCosts_EuroPerMin
        {
            get
            {
                return (ElectricalCost_EuroPerHour + StaffCosts_EuroPerHour * BoostFactor) / 60;
            }
        }

        // Стоимость отходов при изменении ширины пленки, евро
        // Стоимость отходов при изменении рецептуры пленки, евро
        // Стоимость отходов при изменении кампании, евро
        public float WastesCostsWhileRetargeting_Euro(float wasteMass)
        {
            return wasteMass * GranulateCosts_EuroPerKg;
        }

        // Стоимость перенастройки при изменении ширины пленки, евро
        // Стоимость перенастройки при изменении рецептуры пленки, евро
        // Стоимость перенастройки при изменении кампании, евро
        public float RetargetingCosts(float retargetingTime)
        {
            return retargetingTime * EquipmentCosts_EuroPerMin;
        }

        // Суммарная стоимость отходов и перенастройки при изменении ширины пленки, евро
        // Суммарная стоимость отходов и перенастройки при изменении рецептуры пленки, евро
        // Суммарная стоимость отходов и перенастройки при изменении кампании, евро
        public float SummaryCostOfRetargetingAndWaste(float wasteMass, float retargetingTime)
        {
            return WastesCostsWhileRetargeting_Euro(wasteMass) + RetargetingCosts(retargetingTime);
        }
    }
}
