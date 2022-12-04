using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartBall.UserControls;

namespace SmartBall
{
    /// <summary>
    /// Класс исполнителя
    /// Конструктор принимает:
    /// data - буквы, с которыми будет работать исполнитель
    /// tokens - массив команд (<, >, *, !). формат:
    /// - >[число] вправо на [число] символов
    /// - <[число] то же самое только влево
    /// - * - добавляет к результату букву с той позиции, в которой находится исполнитель
    /// - ! - завершает работу, далее метод Next() не будет ни на что влиять
    /// пример списка токенов: ">1*<2****>1*<1*!"
    /// Метод Next() выполняет следующую команду из списка tokens
    /// Результат работы можно получать после каждого вызова Next() через свойство Result,
    /// свойство IsStopped - флаг завершения работы исполнителя, изначально false (то есть еще не выполнил весь алгоритм)
    /// Выход за границы data порождает исключение, как и нераспознанные символы
    /// </summary>
    class CommandExecutor //научить работать с пустыми строками
    {
        private enum TokenType
        {
            TokenRightArrow,
            TokenLeftArrow,
            TokenDigit,
            TokenStar,
            TokenUndefined
        }

        private enum CommandType
        {
            CommandRight,
            CommandLeft,
            CommandRead,
            CommandUndefined
        }

        private string result = "";
        public string Result { get { return result; } }

        private int tokenCursor = 0;
        private Dictionary<int, RulerDelimeter> data;
        private string tokens;
        private int dataCursor;
        public int DataCursor { get { return dataCursor; } }

        private bool isStopped = false;
        public bool IsStopped { get { return isStopped; } }

        private CommandType currentCommand = CommandType.CommandUndefined;
        private TokenType currentToken = TokenType.TokenUndefined;

        public CommandExecutor(Dictionary<int, RulerDelimeter> data, string tokens, int dataCursor)
        {
            this.data = data;
            this.tokens = tokens;
            this.dataCursor = dataCursor;
        }

        public void Next()
        {
            for (; tokenCursor < tokens.Length;)
            {
                switch (tokens[tokenCursor])
                {
                    case '>':
                    case '<':
                        {
                            if (currentCommand == CommandType.CommandRight ||
                                currentCommand == CommandType.CommandLeft ||
                                currentCommand == CommandType.CommandRead ||
                                currentCommand == CommandType.CommandUndefined)
                            {
                                if (tokens[tokenCursor] == '>')
                                {
                                    currentCommand = CommandType.CommandRight;
                                }
                                else
                                {
                                    currentCommand = CommandType.CommandLeft;
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("Ожидается другая команда в позиции " + (tokenCursor + 1).ToString() + "!" );
                            }

                            if (currentToken == TokenType.TokenDigit ||
                                currentToken == TokenType.TokenStar ||
                                currentToken == TokenType.TokenUndefined)
                            {
                                if (tokens[tokenCursor] == '>')
                                {
                                    currentToken = TokenType.TokenRightArrow;
                                }
                                else
                                {
                                    currentToken = TokenType.TokenLeftArrow;
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("Ожидается другой символ в позиции " + (tokenCursor + 1).ToString() + "!" );
                            }

                            tokenCursor++;

                            break;
                        }
                    case '*':
                        {
                            result += data[dataCursor].TBox.Text;

                            currentCommand = CommandType.CommandRead;

                            if (currentToken == TokenType.TokenDigit ||
                                currentToken == TokenType.TokenStar ||
                                currentToken == TokenType.TokenUndefined)
                                currentToken = TokenType.TokenStar;
                            else
                            {
                                throw new InvalidOperationException("Ожидается цифра в позиции " + (tokenCursor + 1).ToString() +"!");
                            }

                            tokenCursor++;

                            return;
                        }
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            int j = tokenCursor;

                            for (; j < tokens.Length && Char.IsDigit(tokens[j]); j++) ;

                            int step = Convert.ToInt32(tokens.Substring(tokenCursor, j - tokenCursor));

                            if (currentCommand == CommandType.CommandRight)
                            {
                                dataCursor += step;
                            }
                            else
                            {
                                dataCursor -= step;
                            }

                            tokenCursor = j;

                            if (dataCursor < 0 || dataCursor >= data.Count)
                                throw new IndexOutOfRangeException("Стою на месте!");

                            currentToken = TokenType.TokenDigit;

                            return;
                        }
                    case '!':
                        {
                            if (currentToken != TokenType.TokenDigit &&
                                currentToken != TokenType.TokenStar &&
                                currentToken != TokenType.TokenUndefined)
                                throw new InvalidOperationException("Ожидается другой символ в позиции " + (tokenCursor + 1).ToString() + "!");

                            isStopped = true;

                            return;
                        }
                    default:
                        throw new InvalidOperationException("Ожидается другой символ в позиции " + (tokenCursor + 1).ToString() + "!");
                }
            }

            isStopped = true;
        }
    }
}
