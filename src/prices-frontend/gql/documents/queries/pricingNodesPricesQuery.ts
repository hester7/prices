import gql from "graphql-tag";

export default gql`
    query pricingNodesPrices($ids: [ID!]!, $span: ChangeSpan!) {
        pricingNodesById(ids: $ids) {
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
