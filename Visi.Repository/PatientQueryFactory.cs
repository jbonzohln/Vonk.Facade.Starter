using System;
using Hl7.Fhir.Model;
using Microsoft.EntityFrameworkCore;
using Visi.Repository.Models;
using Vonk.Core.Common;
using Vonk.Core.Repository;
using Vonk.Core.Repository.ResultShaping;
using Vonk.Facade.Relational;

namespace Visi.Repository;

public class PatientQuery : RelationalQuery<Child>
{
}

public class PatientQueryFactory : VisiQueryFactory<Child, PatientQuery>
{
    public PatientQueryFactory(DbContext onContext) : base(nameof(Patient), onContext)
    {
    }

    public override PatientQuery AddValueFilter(string parameterName, TokenValue value)
    {
        switch (parameterName)
        {
            case VonkConstants.ParameterCodes.Id:
            {
                if (!long.TryParse(value.Code, out var patientId))
                    throw new ArgumentException("Patient Id must be an integer value.");
                return PredicateQuery(vp => vp.ChildId == patientId);
            }
            case "identifier":
                return PredicateQuery(vp => vp.ChildId.ToString() == value.Code);
            case "gender":
                return value.Code switch
                {
                    "male" => PredicateQuery(p => p.Sex == 'M'),
                    "female" => PredicateQuery(p => p.Sex == 'F'),
                    _ => PredicateQuery(p => p.Sex != 'M' && p.Sex != 'F')
                };
            default:
                return base.AddValueFilter(parameterName, value);
        }
    }

    public override PatientQuery AddValueFilter(string parameterName, MissingValue value)
    {
        var isMissing = value.IsMissing; // true

        switch (parameterName)
        {
            //_id is a bit contrived, as Id is never null in Visi, so if isMissing = true, return false, otherwise return true for every record. 
            case "_id": return PredicateQuery(p => !isMissing);
            //This is a more real example:
            case "identifier": return PredicateQuery(p => p.ChildId == null == isMissing);
            default:
                throw new ArgumentException(
                    $"Filtering for missing values using parameter {parameterName} is not supported.");
        }
    }

    public override PatientQuery AddValueFilter(string parameterCode, DateTimeValue value)
    {
        switch (parameterCode)
        {
            case "birthdate":
            {
                switch (value.Operator)
                {
                    case ComparisonOperator.GT:
                    case ComparisonOperator.STARTS_AFTER:
                        return PredicateQuery(p => p.BirthDateTime > value.Period.UpperBound);
                    case ComparisonOperator.GTE:
                        return PredicateQuery(p => p.BirthDateTime >= value.Period.LowerBound);
                    case ComparisonOperator.LT:
                    case ComparisonOperator.ENDS_BEFORE:
                        return PredicateQuery(p => p.BirthDateTime < value.Period.LowerBound);
                    case ComparisonOperator.LTE:
                        return PredicateQuery(p => p.BirthDateTime <= value.Period.UpperBound);
                    case ComparisonOperator.EQ:
                        return PredicateQuery(p =>
                            value.Period.LowerBound <= p.BirthDateTime && p.BirthDateTime <= value.Period.UpperBound);
                    default:
                        throw new NotSupportedException();
                }
            }
            default:
                throw new NotSupportedException();
        }
    }

    protected override PatientQuery AddResultShape(SortShape sort)
    {
        switch (sort.ParameterCode)
        {
            case "_id": return SortQuery(sort, p => p.ChildId);
            case "identifier": return SortQuery(sort, p => p.ChildId);
            default:
                throw new ArgumentException($"Sorting on {sort.ParameterCode} is not supported.");
        }
    }
}