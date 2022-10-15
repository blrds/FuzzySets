using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy.Models
{
    internal class Core
    {
        private static Alpha Approxima(double Slice, Alpha p, Alpha n)
        {
            Alpha answer = new Alpha();
            answer.Slice = Slice;
            if (p.Less == n.Less) answer.Less = p.Less;
            else
            {
                double a = (p.Slice - n.Slice) / (p.Less - n.Less);
                double b = p.Slice - p.Less * ((p.Slice - n.Slice) / (p.Less - n.Less));
                double x = (Slice - b) / a;
                answer.Less = x;
            }
            if (p.Greater == n.Greater) answer.Greater = n.Greater;
            else
            {
                double a = (p.Slice - n.Slice) / (p.Greater - n.Greater);
                double b = p.Slice - p.Greater * ((p.Slice - n.Slice) / (p.Greater - n.Greater));
                double x = (Slice - b) / a;
                answer.Greater = x;
            }
            return answer;
        }
        private static Tuple<List<Alpha>, List<Alpha>> MakeThemEqual(List<Alpha> A, List<Alpha> B)
        {
            var aa = A.OrderBy(x => x.Slice).ToList();
            var bb = B.OrderBy(x => x.Slice).ToList();
            var aa1 = new List<Alpha>();
            aa1.AddRange(aa);
            foreach (var a in aa1)
            {
                if (bb.Any(x => x.Slice == a.Slice))
                {
                    bb.Remove(bb.Where(x => x.Slice == a.Slice).First());
                    aa.Remove(a);
                }
            }
            var newItems = new List<Alpha>();
            foreach (var a in aa)
                for (int i = 0; i < B.Count - 1; i++)
                    if (B[i].Slice < a.Slice && B[i + 1].Slice > a.Slice)
                        newItems.Add(Approxima(a.Slice, B[i], B[i + 1]));
            B.AddRange(newItems);
            newItems = new List<Alpha>();
            foreach (var a in bb)
                for (int i = 0; i < A.Count - 1; i++)
                    if (A[i].Slice < a.Slice && A[i + 1].Slice > a.Slice)
                        newItems.Add(Approxima(a.Slice, A[i], A[i + 1]));
            A.AddRange(newItems);
            return new Tuple<List<Alpha>, List<Alpha>>(A.OrderBy(x=>x.Slice).ToList(), B.OrderBy(x=>x.Slice).ToList());
        }
        public static List<Alpha> Sum(List<Alpha> A, List<Alpha> B)
        {
            var t = MakeThemEqual(A, B);
            var aa = t.Item1;
            var bb = t.Item2;
            List<Alpha> answer = new List<Alpha>();
            for (int i = 0; i < aa.Count; i++)
                answer.Add(new Alpha(aa[i].Slice, aa[i].Less + bb[i].Less, aa[i].Greater + bb[i].Greater));
            return answer;
        }
        public static List<Alpha> Sub(List<Alpha> A, List<Alpha> B)
        {
            var t = MakeThemEqual(A, B);
            var aa = t.Item1;
            var bb = t.Item2;
            List<Alpha> answer = new List<Alpha>();
            for (int i = 0; i < aa.Count; i++)
                answer.Add(new Alpha(aa[i].Slice, aa[i].Less - bb[i].Greater, aa[i].Greater - bb[i].Less));
            return answer;
        }
        public static List<Alpha> Mult(List<Alpha> A, List<Alpha> B)
        {
            var t = MakeThemEqual(A, B);
            var aa = t.Item1;
            var bb = t.Item2;
            List<Alpha> answer = new List<Alpha>();
            for (int i = 0; i < aa.Count; i++)
                answer.Add(new Alpha(aa[i].Slice, aa[i].Less * bb[i].Less, aa[i].Greater * bb[i].Greater));
            return answer;
        }
        public static List<Alpha> Div(List<Alpha> A, List<Alpha> B)
        {
            var t = MakeThemEqual(A, B);
            var aa = t.Item1;
            var bb = t.Item2;
            List<Alpha> answer = new List<Alpha>();
            for (int i = 0; i < aa.Count; i++)
                if (bb[i].Less == 0 || bb[i].Greater == 0) throw new ArgumentException("Деление на 0");
                else answer.Add(new Alpha(aa[i].Slice, aa[i].Less / bb[i].Greater, aa[i].Greater / bb[i].Less));
            return answer;
        }
        public static List<bool> Comp(List<Alpha> A, List<Alpha> B)
        {
            var answer = new List<bool>();
            double sumA=0, sumB = 0;
            foreach (var a in A)
            {
                sumA += (a.Less + a.Greater);
            }
            sumA /= A.Count;
            foreach (var a in B)
            {
                sumB += (a.Less + a.Greater);
            }
            sumB/= B.Count;
            answer.Add(sumA > sumB);
            answer.Add(sumA >= sumB);
            answer.Add(sumA < sumB);
            answer.Add(sumA <= sumB);
            answer.Add(sumA == sumB);
            answer.Add(sumA != sumB);
            return answer;
        }
    }
}
