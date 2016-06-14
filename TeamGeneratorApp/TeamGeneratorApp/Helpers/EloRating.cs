using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Helpers
{
    public class EloRating
    {
        public double PointsEarned1 { get; set; }
        public double PointsEarned2 { get; set; }

        public double FinalRating1 { get; set; }
        public double FinalRating2 { get; set; }

        public EloRating(double CurrentRating1, double CurrentRating2, bool draw, bool firstWins, double diff = 400, double kFactor = 50)
        {
            /*
            double CurrentR1 = 1500.0;
            double CurrentR2 = 1500.0;
            */

            double E = 0;

            if (!draw)
            {
                if (firstWins)
                {
                    E = kFactor - Math.Round(1 / (1 + Math.Pow(10, ((CurrentRating2 - CurrentRating1) / diff))) * kFactor);
                    FinalRating1 = CurrentRating1 + E;
                    FinalRating2 = CurrentRating2 - E;
                }
                else
                {
                    E = kFactor - Math.Round(1 / (1 + Math.Pow(10, ((CurrentRating1 - CurrentRating2) / diff))) * kFactor);
                    FinalRating1 = CurrentRating1 - E;
                    FinalRating2 = CurrentRating2 + E;
                }
            }
            else
            {
                if (CurrentRating1 == CurrentRating2)
                {
                    FinalRating1 = CurrentRating1;
                    FinalRating2 = CurrentRating2;
                }
                else
                {
                    if (CurrentRating1 > CurrentRating2)
                    {
                        E = (kFactor - Math.Round(1 / (1 + Math.Pow(10, ((CurrentRating1 - CurrentRating2) / diff))) * kFactor)) - (kFactor - Math.Round(1 / (1 + Math.Pow(10, ((CurrentRating2 - CurrentRating1) / diff))) * kFactor));
                        FinalRating1 = CurrentRating1 - E;
                        FinalRating2 = CurrentRating2 + E;
                    }
                    else
                    {
                        E = (kFactor - Math.Round(1 / (1 + Math.Pow(10, ((CurrentRating2 - CurrentRating1) / diff))) * kFactor)) - (kFactor - Math.Round(1 / (1 + Math.Pow(10, ((CurrentRating1 - CurrentRating2) / diff))) * kFactor));
                        FinalRating1 = CurrentRating1 + E;
                        FinalRating2 = CurrentRating2 - E;
                    }
                }
            }
            PointsEarned1 = FinalRating1 - CurrentRating1;
            PointsEarned2 = FinalRating2 - CurrentRating2;

        }
    }
}
