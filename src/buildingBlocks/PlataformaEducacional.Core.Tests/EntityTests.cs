using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using PlataformaEducacional.Core.DomainObjects;
using PlataformaEducacional.Core.Messages;
using System;
using System.Linq;
using PlataformaEducacional.Core.Messages;
using PlataformaEducacional.Core.DomainObjects;
using Xunit;

namespace PlataformaEducacional.Core.Tests
{
    // Test double for Entity (abstract)
    internal class TestEntity : Entity
    {
        public TestEntity() : base() { }
    }

    // Test double for Event (protected ctor)
    internal class TestEvent : Event
    {
        public TestEvent()
        {
            //Timestamp = DateTime.UtcNow;
            MessageType = "Test";
            AggregateId = Guid.NewGuid();
        }
    }

    public class EntityTests
    {
        [Fact]
        public void Ctor_SetsId_NotEmpty()
        {
            var e = new TestEntity();
            Assert.NotEqual(Guid.Empty, e.Id);
        }

        [Fact]
        public void Notificacoes_IsNull_BeforeAddingEvents()
        {
            var e = new TestEntity();
            Assert.Null(e.Notificacoes);
        }

        [Fact]
        public void AddEvent_AddsNotification()
        {
            var e = new TestEntity();
            var ev = new TestEvent();

            e.AddEvent(ev);

            Assert.NotNull(e.Notificacoes);
            Assert.Single(e.Notificacoes);
            Assert.Contains(ev, e.Notificacoes);
        }

        [Fact]
        public void RemoveEvent_RemovesNotification_WhenPresent()
        {
            var e = new TestEntity();
            var ev = new TestEvent();

            e.AddEvent(ev);
            Assert.Single(e.Notificacoes);

            e.RemoveEvent(ev);
            Assert.NotNull(e.Notificacoes);
            Assert.Empty(e.Notificacoes);
        }

        [Fact]
        public void RemoveEvent_DoesNotThrow_WhenNoEvents()
        {
            var e = new TestEntity();
            var ev = new TestEvent();

            // should not throw even if internal list is null
            e.RemoveEvent(ev);
            Assert.Null(e.Notificacoes);
        }

        [Fact]
        public void ClearEvents_RemovesAllNotifications()
        {
            var e = new TestEntity();
            e.AddEvent(new TestEvent());
            e.AddEvent(new TestEvent());
            Assert.Equal(2, e.Notificacoes.Count);

            e.ClearEvents();
            Assert.NotNull(e.Notificacoes);
            Assert.Empty(e.Notificacoes);
        }

        [Fact]
        public void Equals_ReturnsTrue_ForSameReference_AndFalseForNullOrDifferentType()
        {
            var a = new TestEntity();
            Assert.True(a.Equals(a));
            Assert.False(a.Equals(null));
            Assert.False(a.Equals(new object()));
        }

        [Fact]
        public void EqualityOperators_HandleNulls_AndSameId()
        {
            TestEntity a = null!;
            TestEntity b = null!;
            Assert.True(a == b);

            a = new TestEntity();
            Assert.False(a == b);
            Assert.True(a != b);

            b = new TestEntity();
            b.Id = a.Id; // force same identity
            Assert.True(a == b);
            Assert.False(a != b);
        }

        [Fact]
        public void GetHashCode_IsConsistent_ForSameTypeAndId()
        {
            var a = new TestEntity();
            var b = new TestEntity();
            b.Id = a.Id;

            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void ToString_ContainsTypeNameAndId()
        {
            var e = new TestEntity();
            var s = e.ToString();
            Assert.Contains(nameof(TestEntity), s);
            Assert.Contains(e.Id.ToString(), s);
        }
    }
}