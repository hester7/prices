import gql from "graphql-tag";

export default gql`
    query pricingNodePrices($rto: Rtos!, $name: String!, $span: ChangeSpan!) {
        pricingNodeByRtoAndName(rto: $rto, name: $name) {
            id
            name
            regionalTransmissionOperatorId
            pricingNodeTypeId
            prices(span: $span) {
                priceIndexId
                intervalEndTimeUtc
                lmpPrice
            }
        }
    }
`;
