using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace max_sum_in_line
{
    [TestFixture]
    public class LineProcessorTests
    {
        [Test]
        public void It_should_process_single_line()
        {
            var line = new[] {
                "5,10,-5,5.1"
            };

            var processor = LineProcessor.New(line);

            Assert.That(processor.MaxLineNo, Is.EqualTo(0));
            Assert.That(processor.InvalidLineIndexes, Is.Empty);
        }

        [Test]
        public void It_should_process_multiple_lines()
        {
            var line = new[] {
                "5,10,-5,5.1",
                "10,20,-30",
                "5,10,-5,5.1"
            };

            var processor = LineProcessor.New(line);

            Assert.That(processor.MaxLineNo, Is.EqualTo(0));
            Assert.That(processor.InvalidLineIndexes, Is.Empty);
        }

        [Test]
        public void It_should_store_invalid_lines()
        {
            var line = new[] {
                "5,10,-5,5.1",
                "10,b20,-30",
                "5,10a,-5,5.1"
            };

            var processor = LineProcessor.New(line);

            Assert.That(processor.MaxLineNo, Is.EqualTo(0));
            Assert.That(processor.InvalidLineIndexes, Is.EqualTo(new[] {1, 2}));
        }

        [Test]
        public void It_should_throw_on_empty_input()
        {
            var lines = new string[0];

            Assert.That(
                () => LineProcessor.New(lines), Throws.Exception);
        }

        [Test]
        public void It_should_throw_if_all_lines_are_invalid()
        {
            var lines = new[] {
                "5,a10,-5,5.1",
                "..56b",
            };

            Assert.That(
                () => LineProcessor.New(lines), Throws.Exception);
        }

        [Test]
        public void It_should_ignore_line_trailing_comma()
        {
            var lines = new[] {
                "1,2,",
                "5"
            };

            var processor = LineProcessor.New(lines);

            Assert.That(processor.MaxLineNo, Is.EqualTo(1));
            Assert.That(processor.InvalidLineIndexes, Is.Empty);
        }

        [Test]
        public void double_comma_is_invalid()
        {
            var lines = new[] {
                "1,,2",
                "5"
            };

            var processor = LineProcessor.New(lines);

            Assert.That(processor.MaxLineNo, Is.EqualTo(1));
            Assert.That(processor.InvalidLineIndexes, Is.EqualTo(new[] {0}));
        }

        [Test]
        public void just_a_single_comma_in_line_is_invalid()
        {
            var lines = new[] {
                ",",
                "5"
            };

            var processor = LineProcessor.New(lines);

            Assert.That(processor.MaxLineNo, Is.EqualTo(1));
            Assert.That(processor.InvalidLineIndexes, Is.EqualTo(new[] {0}));
        }

        [Test]
        public void It_should_handle_lines_with_single_number()
        {
            var lines = new[] {
                "10",
                "20, 30",
                "40"
            };

            var processor = LineProcessor.New(lines);

            Assert.That(processor.MaxLineNo, Is.EqualTo(1));
            Assert.That(processor.InvalidLineIndexes, Is.Empty);
        }

        [Test]
        public void It_should_handle_dot_in_numbers()
        {
            var lines = new[] {
                "10.1,10.2",
                "10.1,10.3",
                "8.8"
            };

            var processor = LineProcessor.New(lines);

            Assert.That(processor.MaxLineNo, Is.EqualTo(1));
            Assert.That(processor.InvalidLineIndexes, Is.Empty);
        }

        [Test]
        public void On_same_max_It_should_return_the_first_line()
        {
            var lines = new[] {
                "1, 20",
                "10, 20",
                "20, 10",
                "8.8"
            };

            var processor = LineProcessor.New(lines);

            Assert.That(processor.MaxLineNo, Is.EqualTo(1));
            Assert.That(processor.InvalidLineIndexes, Is.Empty);
        }


        [Test]
        public void It_should_process_empty_lines()
        {
            var lines = new[] {
                "1, 20",
                "",
                "20, 10",
                "8.8"
            };

            var processor = LineProcessor.New(lines);

            Assert.That(processor.MaxLineNo, Is.EqualTo(2));
            Assert.That(processor.InvalidLineIndexes, Is.Empty);
        }

        [Test]
        public void It_should_iterate_over_lines_once()
        {
            var numberOfIterations = 0;

            IEnumerable<string> makeSequence()
            {
                numberOfIterations++;
                yield return "10";
                yield return "20";
            }

            var sequence = makeSequence();
            var processor = LineProcessor.New(sequence);

            Assert.That(numberOfIterations, Is.EqualTo(1));

            Assert.That(processor.MaxLineNo, Is.EqualTo(1));
            Assert.That(processor.InvalidLineIndexes, Is.Empty);
        }
    }
}