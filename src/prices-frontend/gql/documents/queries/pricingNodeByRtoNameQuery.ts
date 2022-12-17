import gql from "graphql-tag";

export default gql`
    query pricingNodeByRtoAndName($rto: Rtos!, $name: String!) {
        pricingNodeByRtoAndName(rto: $rto, name: $name) {
            id
            name
            regionalTransmissionOperatorId
            pricingNodeTypeId
            currentPrice
        }
    }
`;
