﻿using LunarDoggo.StartOptions;
using System;

namespace StartOptions.Tests.Mocks.Commands
{
    //Should throw a NotSupportedException
    public class UnrelatedConstructorParameterCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public UnrelatedConstructorParameterCommand([StartOption("option", "o")]string option, int i) 
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should trow an InvalidOperationException
    public abstract class NotACommandCommand
    {
        [StartOptionGroup("group", "g")]
        public NotACommandCommand([StartOption("option", "o")] string option)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should trow an InvalidOperationException
    public abstract class AbstractClassCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public AbstractClassCommand([StartOption("option", "o")]string option)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should trow an InvalidOperationException
    public class GenericClassCommand<T> : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public GenericClassCommand([StartOption("option", "o")] T option)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should throw a NameConflictException
    public class GroupLongNameConflictCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public GroupLongNameConflictCommand([StartOption("option", "o")]string option)
        { }

        [StartOptionGroup("group", "gr")]
        public GroupLongNameConflictCommand()
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should throw a NameConflictException
    public class GroupShortNameConflictCommand : IApplicationCommand
    {
        [StartOptionGroup("group1", "g")]
        public GroupShortNameConflictCommand([StartOption("option", "o")] string option)
        { }

        [StartOptionGroup("group2", "g")]
        public GroupShortNameConflictCommand()
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should throw a NameConflictException
    public class OptionLongNameConflictCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public OptionLongNameConflictCommand([StartOption("option", "o")] string option1,
                                              [StartOption("option", "op")] string option2)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should throw a NameConflictException
    public class OptionShortNameConflictCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public OptionShortNameConflictCommand([StartOption("option1", "o")] string option1,
                                              [StartOption("option2", "o")] string option2)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should throw a NameConflictException
    public class GroupOptionLongNameConflictCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public GroupOptionLongNameConflictCommand([StartOption("group", "o")] string option)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should throw a NameConflictException
    public class GroupOptionShortNameConflictCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public GroupOptionShortNameConflictCommand([StartOption("option", "g")] string option)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should throw a NameConflictException
    public class GroupGrouplessOptionLongNameConflictCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public GroupGrouplessOptionLongNameConflictCommand([StartOption("group", "o", IsGrouplessOption = true)] string option)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should throw a NameConflictException
    public class GroupGrouplessOptionShortNameConflictCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public GroupGrouplessOptionShortNameConflictCommand([StartOption("group", "o", IsGrouplessOption = true)] string option)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should throw a NameConflictException
    public class OptionGrouplessOptionLongNameConflictCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public OptionGrouplessOptionLongNameConflictCommand([StartOption("option", "o1")] string option1,
                                                            [StartOption("option", "o2", IsGrouplessOption = true)] string option2)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should throw a NameConflictException
    public class OptionGrouplessOptionShortNameConflictCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public OptionGrouplessOptionShortNameConflictCommand([StartOption("option1", "o")] string option1,
                                                             [StartOption("option2", "o", IsGrouplessOption = true)] string option2)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }

    //Should throw a NameConflictException
    public class HelpOptionNameConflictCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public HelpOptionNameConflictCommand([StartOption("help", "o")] string option)
        { }

        public bool Execute()
        {
            throw new NotImplementedException();
        }
    }
}
