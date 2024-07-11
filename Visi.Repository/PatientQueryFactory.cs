using System;
using Hl7.Fhir.Model;
using Microsoft.EntityFrameworkCore;
using Visi.Repository.Models;
using Vonk.Core.Common;
using Vonk.Core.Repository;
using Vonk.Core.Repository.ResultShaping;
using Vonk.Facade.Relational;

namespace Visi.Repository;

public class PatientQuery : RelationalQuery<Child>;

public class PatientQueryFactory(DbContext onContext)
    : VisiQueryFactory<Child, PatientQuery>(nameof(Patient), onContext)
{
    public override PatientQuery AddValueFilter(string parameterName, TokenValue value)
    {
        switch (parameterName)
        {
            case VonkConstants.ParameterCodes.Id:
            {
                if (!ulong.TryParse(value.Code, out var patientId))
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

        return parameterName switch
        {
            "_id" => PredicateQuery(p => !isMissing),
            //This is a more real example:
            "identifier" => PredicateQuery(p => p.ChildId == null == isMissing),
            _ => throw new ArgumentException(
                $"Filtering for missing values using parameter {parameterName} is not supported.")
        };
    }

    public override PatientQuery AddValueFilter(string parameterCode, DateTimeValue value)
    {
        return parameterCode switch
        {
            "birthdate" => value.Operator switch
            {
                ComparisonOperator.GT or ComparisonOperator.STARTS_AFTER => PredicateQuery(p =>
                    p.BirthDateTime > value.Period.UpperBound),
                ComparisonOperator.GTE => PredicateQuery(p => p.BirthDateTime >= value.Period.LowerBound),
                ComparisonOperator.LT or ComparisonOperator.ENDS_BEFORE => PredicateQuery(p =>
                    p.BirthDateTime < value.Period.LowerBound),
                ComparisonOperator.LTE => PredicateQuery(p => p.BirthDateTime <= value.Period.UpperBound),
                ComparisonOperator.EQ => PredicateQuery(p =>
                    value.Period.LowerBound <= p.BirthDateTime && p.BirthDateTime <= value.Period.UpperBound),
                _ => throw new NotSupportedException()
            },
            _ => throw new NotSupportedException()
        };
    }

    protected override PatientQuery AddResultShape(SortShape sort)
    {
        return sort.ParameterCode switch
        {
            "_id" => SortQuery(sort, p => p.ChildId),
            "identifier" => SortQuery(sort, p => p.ChildId),
            _ => throw new ArgumentException($"Sorting on {sort.ParameterCode} is not supported.")
        };
    }
}