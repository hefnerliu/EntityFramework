// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;

namespace Microsoft.Data.Entity.ChangeTracking.Internal
{
    public class InternalMixedEntityEntry : InternalEntityEntry
    {
        private readonly object[] _shadowValues;

        public InternalMixedEntityEntry(
            [NotNull] IStateManager stateManager,
            [NotNull] IEntityType entityType,
            [NotNull] IEntityEntryMetadataServices metadataServices,
            [NotNull] object entity)
            : base(stateManager, entityType, metadataServices)
        {
            Entity = entity;
            _shadowValues = new object[entityType.ShadowPropertyCount()];
        }

        public InternalMixedEntityEntry(
            [NotNull] IStateManager stateManager,
            [NotNull] IEntityType entityType,
            [NotNull] IEntityEntryMetadataServices metadataServices,
            [NotNull] object entity,
            [NotNull] IValueReader valueReader)
            : base(stateManager, entityType, metadataServices)
        {
            Entity = entity;
            _shadowValues = ExtractShadowValues(valueReader);
        }

        [NotNull]
        public override object Entity { get; }

        protected override object ReadPropertyValue(IPropertyBase propertyBase)
        {
            var property = propertyBase as IProperty;

            return property == null || !property.IsShadowProperty
                ? base.ReadPropertyValue(propertyBase)
                : _shadowValues[property.GetShadowIndex()];
        }

        protected override void WritePropertyValue(IPropertyBase propertyBase, object value)
        {
            var property = propertyBase as IProperty;

            if (property == null
                || !property.IsShadowProperty)
            {
                base.WritePropertyValue(propertyBase, value);
            }
            else
            {
                _shadowValues[property.GetShadowIndex()] = value;
            }
        }

        private object[] ExtractShadowValues(IValueReader valueReader)
        {
            var shadowValues = new object[EntityType.ShadowPropertyCount()];

            foreach (var property in EntityType.GetProperties().Where(property => property.IsShadowProperty))
            {
                shadowValues[property.GetShadowIndex()] = MetadataServices.ReadValueFromReader(valueReader, property);
            }

            return shadowValues;
        }
    }
}
