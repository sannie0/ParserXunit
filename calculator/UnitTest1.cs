using FluentAssertions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

namespace testing
{
//деление (проверка на 0!)
//умножение
//int max value+int max value-обертка для переполнения checker
//регулярные выражения, разделение строк
//вавлидация
    public class Calculator_Test
    {
        Calculator _calculator;
        [SetUp]
        public void Setup()
        {
            _calculator = new Calculator("");
        }
        
        [Test]
        public void Return_Result_IfArithmeticExpression1()
        {
            string test = "1,2+1,2"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("2,4"));
        }

        [Test]
        public void Return_Result_IfArithmeticExpression2()
        {
            string test = "(1+8)/3"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("3"));
        }
        
        [Test]
        public void Return_Result_IfArithmeticExpression3()
        {
            string test = "((1+8)/3)-2*3"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("-3"));
        }
        
        [Test]
        public void Return_Result_IfArithmeticExpression4()
        {
            string test = "((1+8)/3)-2*3*(7-2)"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("-27"));
        }
        
        [Test]
        public void Return_Result_IfArithmeticExpression5()
        {
            string test = "((17333-333)*(500-499))/1000"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("17"));
        }
        
        [Test]
        public void Return_Result_IfArithmeticExpression6()
        {
            string test = "(((17333-333)*(500-499))/1000)*(((17333-333)*(500-499))/1000)*(((17333-333)*(500-499))/1000)"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("4913"));
        }
        
        /*[Test]
        public void Return_Result_IfArithmeticExpression7()
        {
            string test = "100000*10000000000000"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("1000000000000000000"));
        }*/
        
        [Test]
        public void Return_Result_IfArithmeticExpression8()
        {
            string test = "(1,2+1,2)*(1,2+1,2)*(1,2+1,2)"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("13,8"));
        }
        
        
        [Test]
        public void Return_Result_IfArithmeticExpression9()
        {
            string test = "(1,2+1,2)*(1,2+1,2)*(1,2+1,2)"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("13,8"));
        }
        
        [Test]
        public void Return_Result_IfArithmeticExpression10()
        {
            string test = "1+2+3+4+5+6+7+3"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("31"));
        }
        
        [Test]
        public void Return_Result_IfArithmeticExpression11()
        {
            string test = "1/0";
            Assert.Throws<DivideByZeroException>((() => _calculator.StartParse(test)));
        }

        [Test]
        public void Return_Result_IfArithmeticExpression12()
        {
            string test = "(-4-4)*(-5)"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("40"));
        }
        
        [Test]
        public void Return_Result_IfArithmeticExpression13()
        {
            string test = "(-4-4)*(-4-(-7)+(-8--5))"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("-0"));
        }
        
        [Test]
        public void Return_Result_IfArithmeticExpression14()
        {
            string test = "(-4-4)*(-4-(-7)+(-8--5))+(-7)"; 
            Assert.That(_calculator.StartParse(test), Is.EqualTo("-7"));
        }
        
        [Test]
        public void Return_ShouldReturn_IfTheNumberIsPositive()
        {
            string test = "5";
            int result = _calculator.ReturnP(test);
            result.Should().BePositive();
        }

        [Test]
        public void Return_ShouldReturn_IfTheNumberNegitive()
        {
            string test = "-5";
            int result = _calculator.ReturnN(test);
            result.Should().BeNegative();
        }

        [Test]
        public void Return_ShouldReturn_IfTheNumberDouble()
        {
            string test = "4,2";
            double result = _calculator.ReturnD(test);
            Assert.That(result, Is.EqualTo(4.2));
        }
        
        [Test]
        public void Return_ShouldReturn_IfEmpty()
        {
            string test = "  ";
            string result = _calculator.ReturnE(test);
            result.Should().BeEmpty();
        }
    }

    class Calculator
    {
        private int position;
        
        public Calculator(string input)
        {
            this.position = 0;
        }
        public string StartParse(string test)
        {
            position = 0;
            return ProcE(test).ToString();
        }
        public double ProcE(string test)
        {
            bool negative = false;

            SkipSpace(test);

            // Учёт отрицательных чисел
            if (test[position] == '-')
            {
                negative = true;
                position++;
            }

            double x = ProcT(test);

            if (negative)
            {
                x = -x;
            }

            SkipSpace(test);

            while (position < test.Length && (test[position] == '+' || test[position] == '-'))
            {
                char operation = test[position];
                position++;
                SkipSpace(test);

                double temp = ProcT(test);

                if (operation == '+')
                {
                    x += temp;
                }
                else
                {
                    x -= temp;
                }

                SkipSpace(test);
            }

            return Math.Round(x, 1);
        }

        public double ProcT(string test)
        {
            double x = ProcM(test);
            SkipSpace(test);

            while (position < test.Length && (test[position] == '*' || test[position] == '/'))
            {
                char operation = test[position];
                position++;

                if (operation == '*')
                {
                    x *= ProcM(test);
                }
                else
                {
                    double tempM = ProcM(test);
                    if (tempM == 0)
                    {
                        throw new DivideByZeroException();
                    }

                    x /= tempM;
                }

                SkipSpace(test);
            }

            return x;
        }

        public double ProcM(string test)
        {
            double x;
            SkipSpace(test);

            if (test[position] == '(' && position < test.Length)
            {
                position++;
                x = ProcE(test);
                if (position >= test.Length || test[position] != ')')
                {
                    throw new NotImplementedException();
                }
                position++;
            }
            else
            {
                SkipSpace(test);
                if (position < test.Length && (Char.IsDigit(test[position]) || test[position] == '-'))
                {
                    x = ProcC(test);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return x;
        }

        private double ProcC(string test)
        {
            bool negative = false;

            SkipSpace(test);

            // Учёт отрицательных чисел
            if (test[position] == '-')
            {
                negative = true;
                position++;
            }

            string s = "";

            while (position < test.Length && Char.IsDigit(test[position]))
            {
                s += test[position];
                position++;

                if (position < test.Length && test[position] == ',')
                {
                    s += test[position];
                    position++;
                    SkipComma(test);
                }

                while (position < test.Length && Char.IsDigit(test[position]))
                {
                    s += test[position];
                    position++;
                }

                break;
            }

            if (double.TryParse(s, out double result))
            {
                return negative ? -result : result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public int ReturnP(string test)
        {
            int result;
            if (int.TryParse(test, out result) && result >= 0)
            {
                return result;
            }
            throw new NotImplementedException();
        }

        public int ReturnN(string test)
        {
            int result;
            if (int.TryParse(test, out result) && result < 0)
            {
                return result;
            }
            throw new NotImplementedException();
        }

        public double ReturnD(string test)
        {
            double result;
            if (double.TryParse(test, out result))
            {
                return result;
            }
            throw new NotImplementedException();
        }
        public string ReturnE(string test)
        {
            if (string.IsNullOrWhiteSpace(test))
            {
                return string.Empty;
            }
            throw new NotImplementedException();
        }

        public int AddParse(string test)
        {
            int summ = 0;
            List<int> summList = new List<int>();
            foreach (char c in test)
            {
                if (Char.IsDigit(c))
                {
                    summList.Add(int.Parse(c.ToString()));
                }
            }

            foreach (int i in summList)
            {
                summ += i;
            }
            return summ;
        }
        
        public int SubParse(string test)
        {
            
            List<int> subList = new List<int>();
            foreach (char c in test)
            {
                if (Char.IsDigit(c))
                {
                    subList.Add(int.Parse(c.ToString()));
                }
            }
            int sub = subList[0];
            subList.RemoveAt(0);;
            foreach (int i in subList)
            {
                sub -= i;
            }
            return sub;
        }

        public void SkipSpace(string test)
        {
            while (position < test.Length && test[position] == ' ')
            {
                position++;
            }
        }
        
        public void SkipComma(string test)
        {
            while (position < test.Length && test[position] == ',')
            {
                position++;
            }
        }

        
    }
}
