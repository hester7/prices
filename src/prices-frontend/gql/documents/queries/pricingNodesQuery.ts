import gql from "graphql-tag";

export default gql`
    query pricingNodes {
        pricingNodes {
            id
            name
            regionalTransmissionOperatorId
            pricingNodeTypeId
            currentPrice
            change24Hour
        }
    }
`;
