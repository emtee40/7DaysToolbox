﻿namespace SDTD.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Represents a single item.
    /// </summary>
    public class Item : ObservableBase<Item>, IElement
    {
        private Dictionary<String, String> _properties = new Dictionary<String, String>();

        /// <summary>
        /// Create a new <c>Item</c> instance with the specified attributes, belonging to the given collection.
        /// </summary>
        /// <param name="collection">The collection to which the item belongs.</param>
        /// <param name="id">The numeric identifier of the item.</param>
        /// <param name="name">The (untranslated) internal name of the item.</param>
        public Item(ItemCollection collection, UInt32 id, String name) : base(collection, id, name)
        {
            // No additional implementation.
        }

        /// <summary>
        /// Adds a property to the item from its XML description.
        /// </summary>
        /// <param name="drop">A &lt;property&gt; element in a game-compatible items.xml format.</param>
        public void AddProperty(XElement property)
        {
            String name = property.Attribute("name")?.Value;
            String value = property.Attribute("value")?.Value;
            // if (name == null) { throw new ArgumentException("The given property is missing a name."); }
            // TODO: Handle hierarchial Item properties
            if (name == null) { return; }
            this.SetProperty(name, value);
        }

        /// <summary>
        /// Gets the value of the specified property from this item as a string.
        /// </summary>
        /// <param name="name">The name of the property to retrieve.</param>
        /// <returns>The value of the property as a string, or null if the property has the default value.</returns>
        public String GetProperty(String name)
        {
            if (this._properties.ContainsKey(name)) {
                return this._properties[name];
            } else {
                return null;
            }
        }

        /// <summary>
        /// Sets a property on the item, inheriting the default if the value is null.
        /// </summary>
        /// <param name="name">The name of the property to modify.</param>
        /// <param name="value">The value to set the property to, or null to remove the property.</param>
        /// <returns>True if the property was changed, or false if no changes were made.</returns>
        public Boolean SetProperty(String name, String value)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }

            String oldValue = this.GetProperty(name);

            if (this._properties.ContainsKey(name)) {
                if (value == null) {
                    this._properties.Remove(name);
                } else {
                    this._properties[name] = value;
                }
            } else {
                if (value != null) {
                    this._properties.Add(name, value);
                } else {
                    return false;
                }
            }
            if (this.GetProperty(name) == oldValue) {
                return false;
            } else {
                this.OnPropertyChanged(name);
            }
            return true;
        }

        /// <summary>
        /// Generates a game-compatible XML description of the item.
        /// </summary>
        /// <returns>The &lt;item&gt; element representing this item.</returns>
        public XElement ToXElement()
        {
            XElement element = new XElement("item",
                new XAttribute("id", this.ID),
                new XAttribute("name", this.Name));
            foreach (KeyValuePair<String, String> propertyKVP in this._properties.OrderBy(p => p.Key)) {
                XElement propertyElement = new XElement("property",
                    new XAttribute("name", propertyKVP.Key),
                    new XAttribute("value", propertyKVP.Value));
                element.Add(propertyElement);
            }
            return element;
        }
    }
}
